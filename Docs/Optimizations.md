# Optimizations

In order to eek out as much performance, several optimizations have been implemented. As this is meant
to be a teaching tool / clean implementation, telling my thought process should be good.

## Memory Translation Cache

The memory layout of Schoolyard is quite simple. You have _Devices_, which have data that can be addressed, and the _Controller_, which is how they are accessed. This optimization is in the Controller.

In the Controller, the `Read` and `Write` functions take a Gameboy address, from `0x0000` to `0xFFFF`. That's 65355 different addresses.

The read and write functions take that address, and loop through a list of devices to see if that device is mapped to that address. If it is, it return the device, or null, if it couldn't find the device.

The issue is that this is a O(n) operation, for every memory access. This
becomes slow, fast, as memory accesses are quite frequent, even from the
emulator. Early profiling suggested that **33%** of the CPU's time was in `MemoryController::GetMappedDevice`. (Tests were single-threaded, Release build profiling)

While I could just hardcode mappings, that's far less flexable. Instead, I
pre-cache all mappings, as mappings do not change while the game is running.
This reduces all memory accesses to O(1) operations, which is optimal.

Normally, pre-caching an address space isn't feasable, as 32-bit+ address
spaces become unwieldly, and have diminishing performance benefits, with
increasing memory footprints. However, since the Gameboy has a 16-bit address
space, that is small enough that, while Cache lines are a concern, the
performance increase outweights the memory requirements significantly.

## Memory device direct access

TODO: Write