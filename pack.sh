#!/bin/sh

name="BingWallpaper"
d="$name/bin/Release"
zip -j "$name.zip" $d/$name.exe \
    $d/fastjson.dll $d/Ookii.Dialogs.Wpf.dll

f="readme.txt"
cp README.md $f
echo "[Source](https://github.com/yanxyz/$name)" >> $f
zip -ml "$name.zip" $f
