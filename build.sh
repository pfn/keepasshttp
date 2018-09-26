#!/bin/sh

rm KeePassHttp.plgx
rm mono/KeePassHttp.dll
rm -r KeePassHttp/bin
rm -r KeePassHttp/obj
keepass --plgx-create KeePassHttp
msbuild -target:clean KeePassHttp.sln
msbuild -p:Configuration=Release KeePassHttp.sln
cp KeePassHttp/bin/Release/KeePassHttp.dll mono
