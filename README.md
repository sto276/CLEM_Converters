# CLEM_Converters

This is a tool for converting agricultral models to ApsimX files for use with [ApsimNG](https://github.com/APSIMInitiative/ApsimX). It is comprised of three parts, the Model, the Readers and the GUI's.

It has been developed specifically for implementing CLEM models into ApsimNG, however in theory it could be extended to service other models.

## Model
The model is essentially the 'skeleton' of an .apsimx file. It outlines the components required for the file to be valid. Each model is a tree structure, with each node representing one of the models found in ApsimNG. Only a small subset of the models in ApsimNG are presently available as nodes (specifically the ones required for CLEM). The model is provided with data through an interface, which it then uses to automatically output an .apsimx file using JSON serialization. The interface is implemented through the readers.

## Readers
The following file types/model readers are available at present:
  - IAT
  - NABSA
The function of a reader is to be able to parse a data file and provide the information the model needs to generate an .apsimx file. The readers project also contains the "Shared" namespace, which contains methods which are shared between readers (such as error tracking/handling). 

## GUIs
There are two GUI projects, one using Gtk, one using Windows forms. The Gtk UI is considered deprecated, but has not been removed in case there is a need for a cross-platform interface in the future. 

The aim of the UI is to act as an extended file browser, to let the user pick and choose which files they need converted, as well as allow for additional options to be selected. The functionality is basic at present, but is easily extended. 
