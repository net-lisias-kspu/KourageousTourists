# Kourageous Tourists :: Changes

* 2020-1025: 0.5.2.1 (Lisias) for KSP >= 1.3
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
