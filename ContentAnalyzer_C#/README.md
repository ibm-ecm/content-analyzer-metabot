# Content Analyzer C# projects

This repository contains the C# projects to build .dll files required to create the Content Analyzer MetaBot.

## Introduction
IBM Business Automation Content Analyzer is a cloud-based API web service that can help you rapidly accelerate extraction and classification of data in your documents. Content Analyzer can digitize, classify and extract unstructured document content using OCR and PDF text extraction.

This C# solution is using Content Analyzer APIs to submit the document for processing, retrieve the processing status, get the and format the JSON output.


## Basic Components
+ [**ContentAnalyzerMetaBot.sln**](ContentAnalyzerMetaBot.sln) - The C# solution
+ [**MetaBot**](MetaBot) - This folder contains the source code of Content Analyzer API functions.
+ [**Test**](Test) - This folder contains the test project to call the functions.

The .dll files required to create the MetaBot are **MetaBot.dll, NewTonsoft.Json.dll, RestSharp.dll** which can be found in [**MetaBot⁩ ▸ ⁨bin⁩ ▸ ⁨Debug⁩**](MetaBot/bin/Debug) folder.

## To Run
1. Open the ContentAnalyzerMetaBot solution first, navigate to Test project and modify the configPath and imagePath in the test.cs file. 
2. Build the solution.
3. Start Running. 

Please look at the readme file in [**ContentAnalyzer_MetaBot**](ContentAnalyzer_MetaBot) folder for more detail about the config.json file.

## Related Links
1.  Content Analyzer https://www.ibm.com/support/knowledgecenter/SSUM7G/com.ibm.bacanalyzertoc.doc/bacanalyzer_1.0.html
2. IBM RPA https://www.ibm.com/support/knowledgecenter/en/SSMGNY_11.0.0/com.ibm.wbpm.rpa.main.doc/kc-homepage-rpa.html

## Disclaimers
This code is sample code created by IBM Corporation. IBM grants you a nonexclusive copyright license to use this sample code example. This sample code is not part of any standard IBM product and is provided to you solely for the purpose of assisting you in the development of your applications. This example has not been thoroughly tested under all conditions. IBM, therefore cannot guarantee nor may you imply reliability, serviceability, or function of these programs. The code is provided "AS IS", without warranty of any kind. IBM shall not be liable for any damages arising out of your or any other parties use of the sample code, even if IBM has been advised of the possibility of such damages. If you do not agree with these terms, do not use the sample code.

Copyright IBM Corp. 2019 All Rights Reserved.
