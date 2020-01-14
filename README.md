# Kourageus Tourists /L Unofficial

Tourists sometimes should be able to go EVA. Now they can.

Unofficial fork by Lisias.


## In a Hurry

* [Latest Release](https://github.com/net-lisias-kspu/KourageusTourists/releases)
	+ [Binaries](https://github.com/net-lisias-kspu/KourageusTourists/tree/Archive)
* [Source](https://github.com/net-lisias-kspu/KourageusTourists)
* Documentation
	+ [Project's README](https://github.com/net-lisias-kspu/KourageusTourists/blob/master/README.md)
	+ [Install Instructions](https://github.com/net-lisias-kspu/KourageusTourists/blob/master/INSTALL.md)
	+ [Tech Docs](https://github.com/net-lisias-kspu/KourageusTourists/blob/master/Docs/index.md)
	+ [Change Log](./CHANGE_LOG.md)
	+ [TODO](./TODO.md) list


## Description

This tiny mod empowers your every day tourists for much more kourageous adventures. With a little bit of training they finally can disembark the space (and other types of) craft. For their efforts not to be left forgotten, they can picture themselves in the most distant places imaginable. And last, but not least - they're willing to pay some good money for their entertainment.

### Technical Details

This mod temporarily promotes tourists to crew members in order to allow them go EVA. However, actual EVA ability is restricted based on current tourist experience level and vessel situation. These restrictions are configurable with defaults as follows:

| Level | EVA allowed when	 | Jetpack  |
|:-----:|:------------------|:--------:
| 0     | Landed on Kerbin; not moving | No
|1      | Landed or Splashed on Kerbin, Landed on Mün or Minmus; surface speed below 1 m/s | 	No
| 2     | Landed or Splashed anywhere; surface speed below 3 m/s | No
| 3     | Landed or Splashed anywhere or being on stable orbit; no speed restrictions | No
| 4     | Landed or Splashed anywhere or being on stable orbit; no speed restrictions | Yes

Level 5 tourists, if they survived to this, can do basically everything. Tourists gain experience just like regular crew, so for training them to level one, just take them to orbital spaceflight around Kerbin and recover. Level 2 could be obtained by Mün/Minmus landings and this is enough for every possible contract destination. You can also train them onsite using facilities that provide Level up crew function, albeit it does not seem safe enough.

Jetpack fuel is drained if tourist level does not allow using it. Tourists still can not perform things like taking surface samples, collect experiment data or pilot ships, however they can carry stuff if KIS is installed and can take photos of themselves. All kerbals in the scene will notice that and express some emotions depending on their courage and stupidity levels. The photo could be found in standard screenshot directory and looks like Glerina Kerman-Mun-17-03-01-12:06:18.png, i.e. file name contains the name of the kerbal taking photo, planetary body and time when it was taken.  

This mod also adds three new types of contracts that depend on tourists ability to de-board the vessel. 

* Walking on the surface of celestial body
* Taking picture of tourist group when they walk on the surface of celestial body
* Taking picture of tourist group when they stay nearby some point of interest (currently anomalies on Kerbin and Mün; exact location is not given, only some hints; make use of your scanners)
* Needless to say that all contracts require safe recovery of all involved tourists.


### Compatible with

* [EVA Fuel](https://github.com/net-lisias-kspu/EvaFuel)
* [EVA Enhancements](https://github.com/net-lisias-kspu/EvaEnhancements)
* [EVA Follower](https://github.com/net-lisias-kspu/EvaFollower)


## Installation

Detailed installation instructions are now on its own file (see the [In a Hurry](#in-a-hurry) section) and on the distribution file.

### License:

Released under the [MIT License](https://opensource.org/licenses/MIT). See [here](./LICENSE).

Please note the copyrights and trademarks in [NOTICE](./NOTICE).


## UPSTREAM

* [whale_2](https://forum.kerbalspaceprogram.com/index.php?/profile/167015-whale_2/) CURRENT MAINTAINER
	+ [Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/167120-*)
	+ [CurseForge](https://www.curseforge.com/kerbal/ksp-mods/kourageous-tourists)
	+ [SpaceDock](https://spacedock.info/mod/1613/Kourageous%20Tourists)
	+ [GitHub](https://github.com/whale2/KourageousTourists)
