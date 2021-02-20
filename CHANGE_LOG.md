# Kourageous Tourists :: Change Log

* 2021-0220: 0.5.2.2 (Lisias) for KSP >= 1.3
	+ Add Support for every KSP Version since 1.3
		- Support for RealChutes is WorkInProgress, but you can try it nevertheless on KSP 1.3... ]:->
		- For KSP < 1.6, KIS is needed to allow removing Helmets.
	+ A lot of bug fixes
		- Allowing Tourists to remove the helmet when they can EVA as it was allowed using KIS on KSP < 1.6 
		- Surviving KSP's Upgrade(Update?) Pipeline
			- Tourists restrictions are not lost anymore on loading savegames
		- Tourists on External Command Seats (or similar parts) are now correctly handled.
			- Including the ability to take selfies. :) 
		- Trying to synchronise the Kerbal's reactions to the Selfie's shuttle.
		- Pull Requests to the upstream.
			- [#3](https://github.com/whale2/KourageousTourists/pull/3) 
			- [#4](https://github.com/whale2/KourageousTourists/pull/4)
			- [#6](https://github.com/whale2/KourageousTourists/pull/6)
	+ Merging upstream changes and fixes:
		- 0.5.2
			- Sky Diving Contracts only available after buzzing the tower
			- new config options: noSkyDiving & forceTouristsInSandbox
		- 0.5.1
			- bug fixes
		- 0.5.0
			- Sky Diving contracts.
	+ **DEVIATIONS** from the Mainstream
		- Added a new ability: SkyDive
		- Changes on the abilities for each Tourist Level
		- Moving the Kourage.cfg to the `PluginData` folder on the user's installment
			- Automatically created from template at first run.
		- Check [template](https://github.com/net-lisias-kspu/KourageousTourists/blob/master/GameData/net.lisias.ksp/KourageousTourists/PluginData/Kourage.cfg) for changes.
* 2020-1025: 0.5.2.1 (Lisias) for KSP >= 1.3
	+ ***WITHDRAWN*** due lame mistake on packaging 
* 2020-0530: 0.5.2 (whale_2) for KSP 1.9.1
	+ This release fixes some minor bugs, but also makes sky diving contracts unlock only after you've buzzed the tower.
	+ Added two config options:
	+ noSkyDiving - to completely disable skydiving contracts
	+ forceTouristsInSandbox - for enabling the mod in sandbox mode (normally disabled)
* 2020-0528: 0.5.1 (whale_2) for KSP 1.9.1
	+ Fix stupid bug when everyone has left the vessel and contract conditions should be checked.
* 2020-0524: 0.5.0 (whale_2) for KSP 1.9.1
	+ Update for latest KSP and SkyDiving contracts!
	+ That's it - the mod is updated for recent KSP versions (1.7 - 1.9) and now you can enjoy throwing kerbals out of your aircraft or helicopter or even a rocket. Parachute will be deployed automatically. However, you should keep them in the vicinity, otherwise the game won't let them land safely.
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
