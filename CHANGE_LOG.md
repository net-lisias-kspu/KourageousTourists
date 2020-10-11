# Kourageous Tourists :: Change Log

* 2020-0116: 0.4.1.1 (Lisias) for KSP >= 1.4
	+ Replicating [fix](https://github.com/whale2/KourageousTourists/pull/2) from [takoss](https://github.com/takoss)
		- Just found it after fixing it myself, but that guy detected and fixed it before me, so I think he deserved be mentioned! :)
	+ Adding KSPe facilities
		- Logging
		- Abstract file system
		- Installmment checks
	+ Changing the debug mode to read the setting from `<PLUGINDATA>/Debug.cfg`
		- It's a deviation from the mainstream. Beware. 
	+ Mitigating (not properly fixing, however) an issue where the Selfies where being taken too soon and not registering the Kerbal's emotions.
		- [Pull request](https://github.com/whale2/KourageousTourists/pull/3) applied to upstream.
	+ Moving the whole thing to `net-lisias-ksp/KourageousTourists` "vendor hierarchy" to prevent clashes to the upstream.
* 2018-0401: 0.4.1 (whale2) for KSP ['1.3.1', '1.3']
	+ Bugfix release.
	+ Fixed stupid bug when contracts was not displayed properly in Mission Control
	+ Fixed situation when everyone left the vessel for excursion.
* 2017-1102: 0.4.0 (whale2) for KSP ['1.3.1', '1.3']
	+ Hotfix release
	+ Bugfixes:
		- Fixed ugly bug in FixedUpdate that was spamming log file.
		- Fixed UFO being on Kerbin instead of Mun
	+ [KourageousTourists-0.4.0.zip](https://github.com/whale2/KourageousTourists/files/1439604/KourageousTourists-0.4.0.zip)
* 2017-1006: 0.3 (whale2) for KSP 1.3 PRE-RELEASE
	+ Release Candidate.
	+ Features:
	+ New contract - some tourists want to get a picture of themselves in front of some unusual objects.
* 2017-0814: 0.2 (whale2) for KSP 1.3 PRE-RELEASE
	+ This is still pre-release, however most things should work.
	+ Fixes:
		- Fixed contract persistence between saves and loads
	+ Features:
		- New contract - some tourists want to get a picture of themselves in some extrakerbestrial environment.
* 2017-0809: 0.1 (whale2) for KSP 1.3 PRE-RELEASE
	+ This is the first pre-release, be warned.
	+ Features:
		- Tourists are granted EVA permission in different situations based on their level
		- When tourist goes on EVA, Jetpack fuel is drained out on lower levels
		- Tourists can take photos; when photo is taken, all kerbals in the scene are likely to express some emotions. Photos can be found in standard 'Screenshot' folder.
		- One contract type is available - it requires to let some tourists to set foot on celestial bodies you have visited
	+ Compatibility:
	+ The mod was tested and found to be compatible with:
		- EVA Follower
		- EVA Enhancements
	+ Issues:
		- EVA Fuel; At the moment fuel, transferred from the vessel, a tourists goes on EVA from, will be lost.
	+ Patch for EVA Fuel was discussed with its current maintainer and PR was sent.
op