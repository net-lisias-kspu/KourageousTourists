source ../CONFIG.inc

PACKAGE="KourageousTourists"
TARGETDIR="net.lisias.ksp/$PACKAGE/"
TARGETBINDIR="$TARGETDIR"
PROJECTSDIR="$PACKAGE/"

DLLS="KourageousTourists"
PD_DLLS="KourageousTourists.KSP.Chute.13.RealChute KourageousTourists.KSP.Chute.14 KourageousTourists.KSP.Chute.14.RealChute"

PROJECT_BRANCH=`git branch | grep \* | cut -d ' ' -f2`
if [ "$PROJECT_BRANCH" == "master" ] ; then
	# Release mode
	PROJECT_STATE=""
else
	# Note: Add a leading dash when this value is present!
	PROJECT_STATE=""
fi

VERSION=$( cat $PACKAGE.version | tr '\n' ' ' | sed -n -E 's/^.*?"VERSION\":\{.*"MAJOR":([0-9]+?),.*?"MINOR":([0-9]+?),.*?"PATCH":([0-9]+?),.*?"BUILD":([0-9]+?).*?\}.*$/\1.\2.\3.\4/p' )
