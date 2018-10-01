# RTI
This code will help a customer decode data collected by a Rowe Technology Inc ADCP and DVL.  It also includes  screening techniques to clean the ADCP data of bad data.

## Codec
The Codec folder contains all the data to decode the ADCP in the different formats.

Start with AdcpCodec.cs

## Communication
Source code to talk to an ADCP with a serial port or Ethernet

Start with AdcpSerialPort.cs or AdcpEthernet.cs


## DotSpatial
Source code to convert Latitude and Longitude to a distance and positions.  Also store GPS data as an object.

## Exporter
Source code to export ADCP data to different formats

## Matlab
Matlab script file to read ensemble data into Matlab

## Models
Store the information about the ADCP and its data into the computer.

#### DataSet 
Contains all the RTB data.

#### PD0
Contains all the PD0 data.

####DVL 
Contains all the DVL NMEA style data.


## Playback
Playback an ensemble file.

Start with FilePlayback.cs or ProjectPlayback.cs

## Post Process
Post process the data.  This will screen and average the data.

#### Average
Contains the averaging of the different data elements

#### Screen
Screen the data for any anomalies.  This will also remove the speed of the boat from the water current data.

#### Vessel Mount
Replace the Heading data with other devices


## Reader
Read in from different sources: file or project.

## Writer
Write the data to different sources: file or project.

