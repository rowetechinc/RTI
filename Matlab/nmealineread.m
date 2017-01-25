function [data,ierr]  =  nmealineread(nline)
%NMEALINEREAD reads an NMEA sentence into a MATLAB structure array
%
%  DATA  =  NMEALINEREAD(NLINE)
%  [DATA,IERR]  =  NMEALINEREAD(NLINE)
%
%  NLINE is an NMEA sentence. DATA is a MATLAB structure array with a
%  varying format, detailed below.
%
%  NMEALINEREAD currently supports the following NMEA sentences:
%                   $GPGGA  Global positioning system fixed data
%                   $GPGLL  Geographic poition [latitude, longitude & time]
%                   $GPVTG  Course over ground and ground speed
%                   $GPZDA  UTC date / time and local time zone offset
%                   $SDDBS  Echo sounder data
%                   $HEHDT  Heading data
%                   $GPHDT  Heading data
%                   $GPGSV  Satellites in View
%                   $PASHR  RT300 proprietary roll and pitch
%
%  $GPGGA gives a DATA structure with the following fields:
%       Time        Day fraction
%       latitude    Decimal latitude north
%       longitude   Decimal longitude east
%
%  $GPGLL gives a DATA structure with the following fields:
%       Time    Day fraction
%       latitude    Decimal latitude north
%       longitude   Decimal longitude east
%
%  $GPVTG gives a DATA structure with the following fields:
%       speed       Speed over ground in knots
%       truecourse  Course over ground in degrees true
%
%  $GPZDA gives a DATA structure with the following fields:
%       Time    Day number
%       offset      The offset (as a fraction of a day) needed to
%                   generate the local time from BODCTime
%
%  $SDDBS gives a DATA structure with the following field:
%       depth       Depth of water below surface (m)
%
%
%  $HEHDT gives a DATA structure with the following field:
%       heading       GPS heading (degrees)
%
%
%  $GPHDT gives a DATA structure with the following field:
%       heading       GPS heading (degrees)
%
%
%  $GPGSV gives a DATA structure with the following field:
%       SatsInView       Satellites in view
%       SNR              SNR  (dB)
%
%
%  $PASHR gives a DATA structure with the following field:
%       Time          Day fraction
%       heading       GPS Heading degrees
%       roll          GPS Roll degrees
%       pitch         GPS Pitch degrees
%       
% 
%
%  IERR returns an error code:
%       -2  -  NMEA string recognised, but function not yet able to read
%              this string
%       -1  -  NMEA string not recognised
%       0   -  No errors
%
% Found at: http://www.mathworks.com/matlabcentral/fileexchange/45616-improved-nmea-line-reader
%
% Adam Leadbetter (alead@bodc.ac.uk) -      2006-Oct-24
% Lex Lombardi (llombardi@dspaceinc.com) -  2014-Feb-19
%                       partially updated to use textscan() for parsing
%                       added support for other fields in GPGGA
%                       added checksum calculator
% Rico Castelo (rcastelo@rowetechinc.com) -  2015-Oct-13
%                       Fixed bugs.
%                       Added new sentences.

ierr  =  0;

%
%%  Set up a list of valid NMEA strings
%
nmea_options  =  [  '$GPGGA'
                    '$GPGLL'
                    '$GPGSA'
                    '$GPGSV'
                    '$GPRMC'
                    '$GPVTG'
                    '$GPZDA'
                    '$SDDBS'
                    '$HEHDT'
                    '$GPHDT'
                    '$PASHR'];
%
%%  Find which string we're dealing with

fields = textscan(nline,'%s','delimiter',',');
%pull the checksum out of the last field and make a new one for it
fields{1}{end+1} = fields{1}{end}(end-1:end);
%cut off the old last field at the chksum delimiter
fields{1}(end-1) = strtok(fields{1}(end-1), '*');
case_t = find(strcmp(fields{1}(1), nmea_options),1);
fields = char(fields{1});
%%  If no valid NMEA string found - quit with an error

if isempty(case_t)
    fprintf(1,'\n\tWarning: Not a valid NMEA string  -  %s ...\n',nline);
    data  =  NaN;
    ierr  =  -1;
    return
end
%
%% read and check the checksum
% Initialise checksum 
checksum = uint8(0);      

%calc it - we drop the leading '$' and trim off the '*' and anything past it
for i_char = 2:(find(nline=='*',1,'last')-1)
    checksum = bitxor(checksum, uint8(nline(i_char))); 
end
checksum = dec2hex(checksum, 2);

%check it
if ((strcmp(fields(end,1:2),checksum))==0)
   %checksum is bad!
    fprintf(1,'\n\tWarning: Checksum Bad  - %s ~= %s',fields(end),checksum);
    data  =  NaN;
    ierr  =  -1;
    return
end

%% TURN ON THE SWITCH!

switch case_t
    case 1 %% GPGGA Read global positioning system fixed data
        
        %first data field is the time
        t_time  =  fields(2,1:end);
        if(isempty(t_time))
            data.BODCTime  =  NaN;
        else
            data.BODCTime  =  datenum(t_time,'HHMMSS') - ...
                floor(datenum(t_time,'HHMMSS'));
        end
        clear t_time;
        
        %next data field is the lat
        t_lat  =  fields(3,1:end);
        data.latitude  =  ...
            str2double(t_lat(1:2)) + (str2double(t_lat(3:end))/60);
        t_latDir = fields(4,1:end);
        if(t_latDir  ==  'S')
            data.latitude  =  data.latitude  *  -1;
        end
        clear t_lat t_latDir;
        
        %then the lon
        t_lon  = fields(5,1:end);
        data.longitude  =  ...
            str2double(t_lon(1:3)) + (str2double(t_lon(4:end))/60);
        t_lonDir = fields(6,1:end);
        if(t_lonDir  ==  'W')
            data.longitude  =  data.longitude  *  -1;
        end
        clear t_lon t_longDir;
        
        %Get the fix quality where 0 = none, 1 = GPS fix, 2 = DGPS fix
        t_fix = fields(7,1:end);
        data.fix  = str2double(t_fix);
        clear t_fix;
        
        %read the number of satellites
        t_sat = fields(8,1:end);
        data.satellites = str2double(t_sat);
        clear t_sat;
        
        %read HDOP
        t_HDOP= fields(9,1:end);
        if isempty(t_HDOP)
            %do nothing
        else
            data.HDOP = str2double(t_HDOP);
        end
        clear t_HDOP;
        
        %Read Altitude
        t_alt = fields(10,1:end);
        if isempty(t_alt)
            %do nothing
        else
            data.altitude = str2double(t_alt);
        end
        clear t_alt;
        
        t_altUnit = fields(11,1:end);
        if (t_altUnit(1)=='M')
            %do nothing
        else
            fprintf(1,'\tWarning: unknown Altitude Unit - %s\n', t_altUnit);
        end
        clear t_altUnit;
        
        % Height of geoid.... meh
        t_altGeo = fields(12,1:end);
        clear t_altGeo;
        t_altGeoUnit = fields(13,1:end);
        if (t_altGeoUnit(1)=='M')
            %do nothing
        else
            fprintf(1,'\tWarning: unknown Height over WGS84 Unit - %s\n', t_altGeoUnit);
        end
        clear t_altGeoUnit;
        
        %Time since DGPS update
        t_DGPSupdate = fields(14,1:end);
        
        
        %Checksum
        
        t_chkSum = fields(15,1:end);
         
        
    case 2 %% GPGLL Read geographic position [lat/lon] and time
        
        t_lat  = fields(2,1:end);
        data.latitude  =  str2double(t_lat(1:2)) + ...
            (str2double(t_lat(3:end)) / 60);
        t_latDir  =  fields(3,1:end);
        if(t_latDir  ==  'S')
            data.latitude  =  data.latitude * -1;
        end
        clear t_lat t_latDir
        
        t_lon  =  fields(4,1:end);
        data.longitude  =  str2double(t_lon(1:3)) + ...
            (str2double(t_lon(4:end)) / 60);
        t_lonDir = fields(5,1:end);
        if(t_lonDir  ==  'W')
            data.longitude  =  data.longitude  *  -1;
        end
        
        if(length(fields) == 7)
            t_time  =  fields(6,1:end);
            data.BODCTime  =  datenum(t_time,'HHMMSS') - ...
                floor(datenum(t_time,'HHMMSS'));
        else
            data.BODCTime  =  NaN;
        end
        
    case 3 %% GPGSA: Read Procision and fix quality information
        %fix and mode info in fields 2&3
        
        t_mode = fields(2,1:end);
        switch t_mode(1)
            case 'M'
                data.fixmode='Manual';
            case 'A'
                data.fixmode='Automatic';
            otherwise
                data.fixmode=NaN;
        end
        clear t_mode
        
        t_mode = fields(3,1:end);
        switch t_mode(1)
            case '1'
                data.fixtype=1; %no fix
                data.fix=0;
            case '2'
                data.fixtype=2; %2D fix
                data.fix=0;
            case '3'
                data.fixtype=3; %3D fix
                data.fix=0;
            otherwise
                data.fixmode=NaN;
        end
        clear t_mode
        
        %% satalite id's in fields 4-15
        t_satID=str2num(fields(4:15,1:end));
        if not(isempty(t_satID))
            data.satellites=t_satID;
        else
            data.satellites=NaN;
        end
              
        %% Dilution of precision's
        t_PDOP = fields(16,1:end);
        if not(isempty(t_PDOP))
            data.PDOP=str2double(t_PDOP);
        else
            data.PDOP=NaN;
        end
        
        t_HDOP = fields(17,1:end);
        if not(isempty(t_PDOP))
            data.HDOP=str2double(t_HDOP);
        else
            data.HDOP=NaN;
        end
        
        t_VDOP = fields(18,1:end);
        if not(isempty(t_VDOP))
            data.VDOP=str2double(t_VDOP);
        else
            data.VDOP=NaN;
        end
        clear t_PDOP t_HDOP t_VDOP
    
        
    case 6 %% GPVTG: Read course over ground and ground speed
        
        t_course  =  fields(2,1:end);
        if(isempty(t_course))
            data.truecourse  =  NaN;
        else
            data.truecourse  =  str2double(t_course);
        end
        
        t_course  =  fields(4,1:end);
        if(isempty(t_course))
            data.magneticcourse  =  NaN;
        else
            data.magneticcourse  =  str2double(t_course);
        end
        
        t_gspeed  =  fields(6,1:end);
        if(isempty(t_gspeed))
            data.groundspeed.knot  =  NaN;
        else
            data.groundspeed.knot  =  str2double(t_gspeed);
        end
        
        t_gspeed  =  fields(8,1:end);
        if(isempty(t_gspeed))
            data.groundspeed.kph  =  NaN;
        else
            data.groundspeed.kph  =  str2double(t_gspeed);
        end
        
        clear t_course t_gspeed;
        
    case 7 %%  Read UTC Date / Time and Local Time Zone Offset

        data.yyyy = fields(5,1:end);
        data.mm = fields(4,1:end);
        data.dd = fields(3,1:end);
        date = datenum([data.yyyy,data.mm,data.dd]);
        time = datenum(nline(8:13),'HHMMSS');
        hourOffset = str2double(nline(29:30));
        minOffset = str2double(nline(32:33));
      
        data.DateTime = date + time - datenum('00:00','HH:MM');
        data.BODCTime  = date + time - floor(time);  
        data.offset  =  (hourOffset + minOffset / 60) / 24;
    case 8 %%  Read echo sounder data
        
        com_mask  =  strfind(nline,',');
        data.depth  =  str2double(...
            nline(com_mask(2) + 1: com_mask(3)-1));
    
    case 9 %%  HEHDT Read Heading
        
        %first data field is the heading
        t_heading  =  fields(2,1:end);
        if(isempty(t_heading))
            data.heading  =  NaN;
        else
            data.heading  = str2double(t_heading);
        end
        clear t_heading;
        
    case 10 %%  GPHDT Read Heading
        
        %first data field is the heading
        t_heading  =  fields(2,1:end);
        if(isempty(t_heading))
            data.heading  =  NaN;
        else
            data.heading  = str2double(t_heading);
        end
        clear t_heading;
        
    case 11 %% PASHR Heading, Pitch and Roll
        %first data field is the time
        t_time  =  fields(2,1:end);
        if(isempty(t_time))
            data.PASHR_BODCTime  =  NaN;
        else
            data.PASHR_BODCTime  =  datenum(t_time,'HHMMSS') - ...
                floor(datenum(t_time,'HHMMSS'));
        end
        clear t_time;
        
        t_heading  =  fields(3,1:end);
        if(isempty(t_heading))
            data.PASHR_heading  =  NaN;
        else
            data.PASHR_heading  = str2double(t_heading);
        end
        clear t_heading;
        
        t_roll  =  fields(5,1:end);
        if(isempty(t_roll))
            data.PASHR_roll  =  NaN;
        else
            data.PASHR_roll  = str2double(t_roll);
        end
        clear t_roll;
        
        t_pitch  =  fields(6,1:end);
        if(isempty(t_pitch))
            data.PASHR_pitch  =  NaN;
        else
            data.PASHR_pitch  = str2double(t_pitch);
        end
        clear t_pitch;
        
        
    case 4 %%  Satellites in view
        
        %Sats in View
        t_satsInView = fields(4,1:end);
        if(isempty(t_satsInView))
            data.SatsInView  =  NaN;
        else
            data.SatsInView  = str2double(t_satsInView);
        end
        
        %SNR
        t_snr = fields(8,1:end);
        if(isempty(t_snr))
            data.SNR  =  NaN;
        else
            data.SNR  = str2double(t_snr);
        end
        
        clear t_satsInView t_snr;
        
    otherwise
        data  =  NaN;
        ierr  =  -2;
        fprintf(1,...
            '\n\tWarning: NMEA reader not yet implemented for this string  -  %s  ...\n %s\n',...
            nline, fields(1,1:end));
end

%%  Tidy up the output structure
%if isstruct(data)==0 
%    data  =  orderfields(data);
%end