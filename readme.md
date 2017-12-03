# CheckTest package

### Introduction

This repository contains two utilities which checks the trx file after a test run to verify: 

1) That the tests have indeed run, otherwise there is no trx file  
2) That the number of tests are higher than a given limit 
3) And checks a log file, that there are no background exceptions tracked in the trx file, which may cause a test to go green (because it only react on foreground thread) while there are background thread exceptions.

The package is uploaded to Nuget.

## Checktest

Upcoming instructions for use

## CheckTestLog

Upcoming instructions for use

