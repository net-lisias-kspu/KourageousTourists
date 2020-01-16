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
