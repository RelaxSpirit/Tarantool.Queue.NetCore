local state    = require('queue.abstract.state')
local parentTube = require('queue.abstract.driver.utube')

local tube = {}
local method = {}

setmetatable(tube ,{__index = parentTube}) 

-- create space
function tube.create_space(space_name, opts)
    return parentTube.create_space(space_name, opts)
end

-- start tube on space
function tube.new(space, on_task_change, opts)
	local max_id
    local parentRaw = parentTube.new(space, on_task_change, opts)
    local commit_func = begin_if_not_in_txn()
    max_id = space.index.task_id:max()
	commit_func()
	
    local self = setmetatable({
        space          = space,
		space_ready_buffer = parentRaw.space_ready_buffer,
        on_task_change = parentRaw.on_task_change,
        parentTubeRaw = parentRaw,
		ready_space_mode   = parentRaw.ready_space_mode,
        opts               = parentRaw.opts,
        task_id = max_id ~= nil and max_id[1] ~= nil and max_id[1] or 0
    }, { __index = method })
    return self
end

-- normalize task: cleanup all internal fields
function method.normalize_task(self, task)
    return self.parentTubeRaw:normalize_task(task)
end

local function commit()
    box.commit()
end

local function empty()
end

-- Start transaction with the correct options, if the transaction is not already running.
local function begin_if_not_in_txn()
    local transaction_opts = {}
    if box.cfg.memtx_use_mvcc_engine then
        transaction_opts = {txn_isolation = 'read-committed'}
    end

    if not box.is_in_txn() then
        box.begin(transaction_opts)
        return commit
    else
        return empty
    end
end

-- put task in space
function method.put(self, data, opts)
	local max_id
    -- Taking the minimum is an implicit transactions, so it is
    -- always done with 'read-confirmed' mvcc isolation level.
    -- It can lead to errors when trying to make parallel 'take' calls with mvcc enabled.
    -- It is hapenning because 'min' for several takes in parallel will be the same since
    -- read confirmed isolation level makes visible all transactions that finished the commit.
    -- To fix it we wrap it with box.begin/commit and set right isolation level.
    -- Current fix does not resolve that bug in situations when we already are in transaction
    -- since it will open nested transactions.
    -- See https://github.com/tarantool/queue/issues/207
    -- See https://www.tarantool.io/ru/doc/latest/concepts/atomic/txn_mode_mvcc/
    local commit_func = begin_if_not_in_txn()

    local max_id = self.space.index.task_id:max()
	
	self.task_id = max_id and max_id[1] > self.task_id and max_id[1] + 1 or self.task_id + 1

    local task = self.space:insert{self.task_id, state.READY, tostring(opts.utube), data}

    commit_func()

    self.on_task_change(task, 'put')
    return task
end
    while true do
        local commit_func = begin_if_not_in_txn()

        local task_ready = self.space_ready_buffer.index.utube:min{opts.utube}
        if task_ready == nil then
            commit_func()
            return nil
        end

        local id = task_ready[1]
        local task = self.space:get(id)
        local take_complete = false

        if task[2] == state.READY then
            local taken = self.space.index.utube:min{state.TAKEN, task[3]}

            if taken == nil or taken[2] ~= state.TAKEN then
                task = self.space:update(id, { { '=', 2, state.TAKEN } })
                self.space_ready_buffer:delete(id)
                take_complete = true
            end
        end

        commit_func()

        if take_complete then
            self.on_task_change(task, 'take')
            return task
        end
    end
end
    for s, task in self.space.index.status:pairs(state.READY,
            { iterator = 'GE' }) do
        if task[2] ~= state.READY then
            break
        end
        -- Taking the minimum is an implicit transactions, so it is
        -- always done with 'read-confirmed' mvcc isolation level.
        -- It can lead to errors when trying to make parallel 'take' calls with mvcc enabled.
        -- It is hapenning because 'min' for several takes in parallel will be the same since
        -- read confirmed isolation level makes visible all transactions that finished the commit.
        -- To fix it we wrap it with box.begin/commit and set right isolation level.
        -- Current fix does not resolve that bug in situations when we already are in transaction
        -- since it will open nested transactions.
        -- See https://github.com/tarantool/queue/issues/207
        -- See https://www.tarantool.io/ru/doc/latest/concepts/atomic/txn_mode_mvcc/
        local commit_func = begin_if_not_in_txn()
        local taken = self.space.index.utube:min{state.TAKEN, task[3]}
        local take_complete = false

        if taken == nil or taken[2] ~= state.TAKEN then
            task = self.space:update(task[1], { { '=', 2, state.TAKEN } })
            take_complete = true
        end

        commit_func()
        if take_complete then
            self.on_task_change(task, 'take')
            return task
        end
    end
end

-- take task
function method.take(self, opts)
	if(opts == nil) then
		return self.parentTubeRaw:take(self)
	end
    local taken

    local commit_func = begin_if_not_in_txn()
	
	taken = self.space.index.utube:min{state.READY, opts.utube}

    if taken ~= nil and taken[2] == state.READY then
        taken = self.space:update(taken[1], { { '=', 2, state.TAKEN } })
		commit_func()
        self.on_task_change(taken, 'take')
        return taken
    end
	commit_func()
end

-- touch task
function method.touch(self, id, ttr)
    return self.parentTubeRaw:touch(id, ttr)
end
    self.space_ready_buffer:delete(id)
    put_next_ready(self, utube)
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

function method.drop(self)
    return parentTube.drop()
end

return tube
