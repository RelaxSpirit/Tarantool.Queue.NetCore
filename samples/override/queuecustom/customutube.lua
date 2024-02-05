local state = require('queue.abstract.state')
local parentTube = require('queue.abstract.driver.utube')


local method = {}
local tube = {}

setmetatable(tube ,{__index = parentTube}) 

-- create space
function tube.create_space(space_name, opts)
   return parentTube.create_space(space_name, opts)
end

function tube.new(space, on_task_change)
    local max_id
    local parentRaw = parentTube.new(space, on_task_change)
    if box.cfg.memtx_use_mvcc_engine and (not box.is_in_txn()) then
        box.begin({txn_isolation = 'read-committed'})
        max_id = space.index.task_id:max()
        box.commit()
    else
        max_id = space.index.task_id:max()
    end
    local self = setmetatable({
        space          = space,
        on_task_change = parentRaw.on_task_change,
        parentTubeRaw = parentRaw,
        task_id = max_id ~= nil and max_id[1] ~= nil and max_id[1] or 0
    }, { __index = method })
    return self
end

function method.take(self, opts)
    local taken

    if box.cfg.memtx_use_mvcc_engine and (not box.is_in_txn()) then
        box.begin({txn_isolation = 'read-committed'})
        taken = self.space.index.utube:min{state.READY, opts.utube}
        box.commit()
    else
        taken = self.space.index.utube:min{state.READY, opts.utube}
    end

    if taken ~= nil and taken[2] == state.READY then
        taken = self.space:update(taken[1], { { '=', 2, state.TAKEN } })
        self.on_task_change(taken, 'take')
        return taken
    end
end


function method.normalize_task(self, task)
    return self.parentTubeRaw:normalize_task(task)
end

-- put task in space
function method.put(self, data, opts)
    local max_id

    if box.cfg.memtx_use_mvcc_engine and (not box.is_in_txn()) then
        box.begin({txn_isolation = 'read-committed'})
        max_id = self.space.index.task_id:max()
        box.commit()
    else
        max_id = self.space.index.task_id:max()
    end

    self.task_id = max and max_id[1] > self.task_id and max_id[1] + 1 or self.task_id + 1

    local task = self.space:insert{self.task_id, state.READY, tostring(opts.utube), data}
    self.on_task_change(task, 'put')
    return task
end

-- touch task
function method.touch(self, id, ttr)
    return self.parentTubeRaw:touch(id, ttl)
end

-- delete task
function method.delete(self, id)
    return self.parentTubeRaw:delete(id)
end

-- release task
function method.release(self, id, opts)
    return self.parentTubeRaw:release(id, opts)
end

-- bury task
function method.bury(self, id)
    return self.parentTubeRaw:bury(id)
end

-- unbury several tasks
function method.kick(self, count)
    return self.parentTubeRaw:kick(count)
end

-- peek task
function method.peek(self, id)
    return self.parentTubeRaw:peek(id)
end

-- get iterator to tasks in a certain state
function method.tasks_by_state(self, task_state)
    return self.parentTubeRaw:tasks_by_state(task_state)
end

function method.truncate(self)
    return self.parentTubeRaw:truncate()
end

-- This driver has no background activity.
-- Implement dummy methods for the API requirement.
function method.start()
    return parentTube.start()
end

function method.stop()
    return parentTube.stop()
end

return tube
