**Toxic Ragers**

[![Nuget](https://img.shields.io/nuget/v/ToxicRagers)](https://www.nuget.org/packages/ToxicRagers/)

A library for the manipulation of various game files.

Game file formats supported...

* Burnout Paradise
  * **.bundle** - Read, Extract
  * **.bom** - Partial read
* Carmageddon
  * **.pix** - Read, Write, Extract
  * **\<carname\>.txt** - Read, Write
  * **\<noncar\>.txt** - Read, Write
  * **opponent.txt** - Read
* Carmageddon 2
  * **.act** - Read, Write
  * **.dat** - Read, Write
  * **.mat** - Read, Write
  * **.wam** - Read, Write
  * **.twt** - Read, Write, Extract
  * **\<carname\>.txt** - Read, Write
  * **\<mapname\>.txt** - Read, Write
  * **opponent.txt** - Read, Write
  * **races.txt** - Read, Write
* Carmageddon PSX
  * code needs tidying
* Carmageddon: Reincarnation
  * **accessory.txt** - Read
  * **accessory.txt** - Read
  * **.light** - Read, Write
  * **.lol** - Read
  * **.minge** - Write
  * **.mt2** - Read, Write
  * **postfx.lol** - Write (sort of, extremely shoddy)
  * **routes.txt** - Read
  * **\<carname\>\setup.lol** - Read, Write
  * **structure.xml** - Read, Write
  * **systemsdamage.xml** - Read, Write
  * **vehiclesetup.cfg** - Read, Write
* Destruction Derby RAW
  * code needs tidying
* Double Steal: Second Clash
  * **.pk** - Read, Extract
  * **.xpr** - Read, Extract (textures)
* Novadrome
  * **.xt2** - Read
* Powerslide
  * code needs tidying
* Stainless Games (formats used in many of their games)
  * **.cnt** - Read, Write
  * **.img** - Read (raw, RLE and huffman), Write (raw, RLE and huffman)
  * **.mdl** - Read, Write
  * **.mtl** - Partial read
  * **.tdx** - Read, Write
  * **.wad** - Read, Extract
  * **.zad** - Read, Write, Extract
* TDR 2000
  * **.h** - Read
  * **.hie** - Read
  * **.mshs** - Read
  * **.pak** - Read, Extract
  * **.tx** - Read
* Vigilante 8: 2nd Offense
  * **xobfbin** - Partial read
  * **.exp** - Read, Extract
* Wreckfest
  * **.bmap** - Read, Write
  * **.scne** - Partial read

Also contains code for...
* Huffman compression/decompression
* LSZZ decompression
* LZ4 compression/decompression
* Big endian binary streams (read and write)
* CRC32 generation
* Reading and writing binary FBX files
* Reading and extracting from Android OBB files
* Reading and writing DDS files
