# MATLAB Script File
Download both files.

Load in MATLAB ReadRTI.m

The script will ask for an ensemble binary file.  Please select an RTB (Rowe Tech Binary) ensemble file.
The file will then be read into MATLAB.
MATLAB will then store each ensemble into a struct.


# MATLAB Exporter
The Pulse Batch Exporter will also export the files into MATLAB files.  There are 2 formats the exporter will export.
 
 #### MATLAB file
   - A MATLAB .m file will be created for every ensemble.  The file name will include the frequency and CEPO index.
   
 #### MATLAB Matrix File
   - A MATLAB .m file will be created for each element of data (East Velocity, North Velocity, Bottom Track Range, ....)
   
 More details on these formats can be found here:
 http://rowetechinc.co/wiki/index.php?title=ADCP_Data_Types