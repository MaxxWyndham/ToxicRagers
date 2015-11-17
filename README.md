Toxic Ragers

Game file formats supported...

* Burnout Paradise
  * **.bundle** - Read, Extract
  * **.bom** - Partial read
* Carmageddon
  * ***car*.txt** - Read (car details, funks, grooves, crush and bounding box data)
  * **.pix** - Read, Extract
* Carmageddon 2
  * **.act** - Read, Write
  * **.dat** - Read, Write
  * **.mat** - Read, Write
  * ***map*.txt** - Partial read
* Carmageddon Mobile
  * **.tdx** - Partial read
* Carmageddon PSX
  * code needs tidying
* Carmageddon: Reincarnation
  * **accessory.txt** - Read
  * **.light** - Read, Write
  * **.lol** - Read
  * **.minge** - Write
  * **.mt2** - Read, Write
  * **postfx.lol** - Write (sort of, extremely shoddy)
  * **routes.txt** - Read
  * ***car\*setup.lol** - Read, Write
  * **structure.xml** - Read, Write
  * **systemsdamage.xml** - Read, Write
  * **.tdx** - Read, Write
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
  * **.wad** - Read, Extract
  * **.zad** - Read, Extract, Write
* TDR 2000
  * **.h** - Read
  * **.hie** - Read
  * **.mshs** - Read
  * **.pak** - Read, Extract
  * **.tx** - Read
* Vigilante 82nd Offense
  * **xobfbin** - Partial read
  * **.exp** - Read, Extract
* Wreckfest
  * **.bmap** - Read
  * **.scne** - Partial read

Also contains code for...
* Huffman compression/decompression
* LSZZ decompression
* LZ4 compression/decompression
* Big endian binary streams (read and write)
* CRC32 generation
* Reading and writing binary FBX files
* Reading and extracting from Android OBB files
* Writing DDS files
