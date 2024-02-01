# Tarantool.Queue.NetCore
NETCore client for working with [Tarantool Queue](https://github.com/tarantool/queue/tree/master) is based on the library [progaudi.tarantool](https://github.com/progaudi/progaudi.tarantool/tree/master).

## .NET Versions
Can be used in projects based on .NET 6.0, .NET 7.0, and .NET 8.0

# Usage
## Standard Tarantool Queue
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
 	1. For [Consume](https://github.com/RelaxSpirit/Tarantool.Queue.NetCore/tree/master/samples/StandardTarantoolQueue/StandardTarantoolQueueReader)
 	2. For [Produce](https://github.com/RelaxSpirit/Tarantool.Queue.NetCore/tree/master/samples/StandardTarantoolQueue/StandardTarantoolQueueWriter)
  3. In projects in the initialization block, replace parameter in function UseTarantoolQueue with your host settings and port.
  4. Run StandardTarantoolQueueReader and StandardTarantoolQueueWriter
  
## Custom Tarantool Queue
* Linux
  1. Copy the [folder of example lua files](https://github.com/RelaxSpirit/Tarantool.Queue.NetCore/tree/master/samples/override) to the directory '/home/[your_Linux_user]/override' of your Linux machine where Tarantool and Tarantool Queue are installed.
  2. Run Tarantool:
     > tarantool - i
  3. Run netBox:
     > tarantool> box.cfg {
      listen = 'host:port'
    }
  4. Start custom Tarantool Queue
     > tarantool> queue = require('queuecustom')
  5. Register custom Tarantool Queue driver
     > tarantool> queue.register_driver('customtube', require('queuecustom.customutube'))
  6. Create test custom tube if need:
     > tarantool> queue.create_tube('queue_test_custom_tube', 'customtube', {if_not_exists = true, temporary = false})
  * Use cases are in projects:
 	1. For [Consume](https://github.com/RelaxSpirit/Tarantool.Queue.NetCore/tree/master/samples/CustomTarantoolQueue/CustomTarantoolQueueReader)
 	2. For [Produce](https://github.com/RelaxSpirit/Tarantool.Queue.NetCore/tree/master/samples/CustomTarantoolQueue/CustomTarantoolQueueWriter)
  7. In projects in the initialization block, replace parameter in function UseTarantoolQueue with your host settings and port.
  8. Run CustomTarantoolQueueReader and CustomTarantoolQueueWriter

The custom Tarantool Queue also supports the full functionality of standard Tarantool Queue queue and tubes.
