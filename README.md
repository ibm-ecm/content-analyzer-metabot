# Business Automation Content Analyzer MetaBot

This repository contains Business Automation Content Analyzer MetaBot which can be installed into IBM Robotic Process Automation (IBM RPA) and C# project to build .dll files required to create the MetaBot.

Content Analyzer MetaBot can be used on IBM RPA with Automation Anywhere client to extract content from electronic or image documents and return structured data that can be used downstream in the IBM RPA logic.

## Introduction
IBM Business Automation Content Analyzer is a cloud-based API web service that can help you rapidly accelerate extraction and classification of data in your documents. Content Analyzer can digitize, classify and extract unstructured document content using OCR and PDF text extraction.

Once you have used the web interface to “train” your Content Analyzer instance to recognize your specific ontology of document classes, Content Analyzer MetaBot can connect and authenticate into Content Analyzer instance via REST APIs, do the calssification and extraction according to your requirements and return the extracted key value pairs in a JSON stream.

This Content Analyzer Metabot is supported in IBM Robotic Process Automation with Automation Anywhere V11.0 

## Basic Components
+ [**ContentAnalyzer_MetaBot**](ContentAnalyzer_MetaBot) - This folder contains the installation ContentAnalyzer.mbot for Automation Anywhere client, and the sample configuration file required when using Content Analyzer MetaBot
+ [**MetaBotBuild_C#**](MetaBotBuild_C%23) - This folder is only for developers and MetaBot creators who want to see the source code. It contains the C# projects which can build the .dll files required to create the MetaBot.

## Installation and Configuration Information
Content Analyzer MetaBot needs to be installed into Automation Anywhere client and adds configuration file before running. Please look at the readme file in [**ContentAnalyzer_MetaBot**](ContentAnalyzer_MetaBot) folder for more detail.

## Related Links
1.  Content Analyzer https://www.ibm.com/support/knowledgecenter/SSUM7G/com.ibm.bacanalyzertoc.doc/bacanalyzer_1.0.html
2. IBM RPA https://www.ibm.com/support/knowledgecenter/en/SSMGNY_11.0.0/com.ibm.wbpm.rpa.main.doc/kc-homepage-rpa.html

## Disclaimers
This code is sample code created by IBM Corporation. IBM grants you a nonexclusive copyright license to use this sample code example. This sample code is not part of any standard IBM product and is provided to you solely for the purpose of assisting you in the development of your applications. This example has not been thoroughly tested under all conditions. IBM, therefore cannot guarantee nor may you imply reliability, serviceability, or function of these programs. The code is provided "AS IS", without warranty of any kind. IBM shall not be liable for any damages arising out of your or any other parties use of the sample code, even if IBM has been advised of the possibility of such damages. If you do not agree with these terms, do not use the sample code.

Copyright IBM Corp. 2019 All Rights Reserved.
