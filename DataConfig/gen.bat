set GEN_CLIENT=dotnet .\Tools\Luban.ClientServer\Luban.ClientServer.dll

%GEN_CLIENT% -j cfg --^
 -d Defines\__root__.xml ^
 --input_data_dir Datas ^
 --output_data_dir ..\Client\Assets\GameRes\ExcelJson ^
 --output_code_dir ..\Client\Assets\\GameScript\Runtime\GenDataDef ^
 --gen_types code_cs_unity_json,data_json ^
 -s all
pause