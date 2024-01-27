# Tarantool.Queue.NetCore
NETCore client for working with Tarantool Queue is based on the library [progaudi.tarantool](https://github.com/progaudi/progaudi.tarantool/tree/master).

## .NET Versions
Can be used in projects based on .NET 6.0, .NET 7.0, and .NET 8.0

# Usage
* Linux
  1. Run Tarantool:
     > tarantool - i
  2. Run netBox:
     > tarantool> box.cfg {
      listen = 'host:port'
    }
  3. Start Tarantool Queue
     > tarantool> queue = require('queue')
  4. Create test tube if need:
     > tarantool> queue.create_tube('queue_test_fifo', 'fifo', {if_not_exists = true, temporary = false})
* Use cases are in projects:
 	1. For [Consume](samples/TarantoolReader)
 	2. For [Produce](samples/TarantoolWriter)
  3. In projects in the initialization block, replace parameter in function UseTarantoolQueue with your host settings and port.
  4. Run TarantoolReader and TarantoolWriter
