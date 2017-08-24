#!/bin/bash
OUT=0;
for D in `find . -type f -name *.csproj`
do
    echo ========================================================
    echo ========================================================
	echo Building: ${D}
	echo ========================================================
	echo ========================================================
    dotnet build "${D}"
    OUT=$(($OUT+$?))
done

exit $OUT;