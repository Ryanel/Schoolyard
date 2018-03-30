# Schoolyard

Schoolyard is an original Gameboy Emulator.
It is being designed in a simple and readable manner.

It is named Schoolyard because that is where Gameboy's used to be played a lot.

Schoolyard is currently under development. It can only run test carts, but fails at some tests.

## Games

* Tetris: Garbled output
* Dr.Mario: Gameplay desyncs, pills merge.

### Tests

* OPUS5: Display
* Blarggs
  * cpu_instrs: Halts
  * 01-special: Fails
  * 02-interrupts: Fails
  * 03-op sp,hl: Passes
  * 04-op r,imm: Fails
  * 05-op rp: Fails
  * 06-ld r,r: Passes
  * 07-jr,jp,call,ret,rst: Passes
  * 08-misc instrs: Passes
  * 09-op r,r: Fails
  * 10-bit ops: Passes
  * 11-op a,(hl): Fails

## Todo's

* Pass all tests
* Comb-through the Opcode table and package any repetitive code

## Failing Opcodes

* DAA
* Timer
* 09
* 19
* 27
* 29
* 88 - 9F
* CE (adc imm8)
* DE (sbc imm8)
