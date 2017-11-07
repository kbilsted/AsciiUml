$version = "0.1.2"
$zipper = "C:\Program Files\7-Zip\7z.exe"

$homie = pwd

# win10-64 bit
dotnet publish -c Release -r win10-x64
cd .\AsciiUmlCore\bin\Release\netcoreapp2.0\win10-x64\publish\
$zipname = $homie\Asciiuml-$version-win10-64bit.zip
& $zipper a -bb0 -bso0 -bsp0 $zipname .
"Creating archive: " + $zipname
cd $homie

# Add more here...
