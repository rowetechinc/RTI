using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Convert the Ensemble object to a PD6 or PD13.
    /// </summary>
    public class EnsToPd6_13Codec
    {

        #region Structs and Classes

        /// <summary>
        /// Store the PD6 and PD13.
        /// </summary>
        public class Pd6_13Data
        {
            /// <summary>
            /// List of all the PD6 strings.
            /// </summary>
            public List<string> Data { get; set; }

            /// <summary>
            /// Initialize the object.
            /// </summary>
            public Pd6_13Data()
            {
                this.Data = new List<string>();
            }
        }

        #endregion

        /// <summary>
        /// Convert the given Ensemble data into PD6 and PD13 strings.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns></returns>
        public Pd6_13Data Encode(DataSet.Ensemble ens)
        {
            Pd6_13Data data = new Pd6_13Data();

            data.Data.Add(EncodeSA(ens));
            data.Data.Add(EncodeTS(ens));
            data.Data.Add(EncodeRA(ens));
            data.Data.Add(EncodeWI(ens));
            data.Data.Add(EncodeWS(ens));
            data.Data.Add(EncodeWE(ens));
            //data.Data.Add(EncodeWD(ens));
            data.Data.Add(EncodeBI(ens));
            data.Data.Add(EncodeBS(ens));
            data.Data.Add(EncodeBE(ens));
            //data.Data.Add(EncodeBD(ens));

            return data;
        }

        /// <summary>
        /// Get the data out of the ensemble to encode the SA message.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <returns>SA string.</returns>
        private string EncodeSA(DataSet.Ensemble ens)
        {
            float heading = 0.0f;
            float pitch = 0.0f;
            float roll = 0.0f;

            if(ens.IsAncillaryAvail)
            {
                heading = ens.AncillaryData.Heading;
                pitch = ens.AncillaryData.Pitch;
                roll = ens.AncillaryData.Roll;
            }
            else if(ens.IsBottomTrackAvail)
            {
                heading = ens.BottomTrackData.Heading;
                pitch = ens.BottomTrackData.Pitch;
                roll = ens.BottomTrackData.Roll;
            }
            else if(ens.IsDvlDataAvail)
            {
                heading = ens.DvlData.Heading;
                pitch = ens.DvlData.Pitch;
                roll = ens.DvlData.Roll;
            }

            return new SA(heading, pitch, roll).Encode();
        }

        /// <summary>
        /// Get the data out of the ensemble to encode the TS message.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <returns>TA string.</returns>
        private string EncodeTS(DataSet.Ensemble ens)
        {
            DateTime dateAndTime = new DateTime();
            byte hundredthSec = 0;
            float salinity = 0.0f;
            float temperature = 0.0f;
            float depthOfTransducer = 0.0f;
            float speedOfSound = 0.0f;
            int BIT = 0;
            RTI.DataSet.DvlDataSet.LeakDetectionOptions leakDetection = RTI.DataSet.DvlDataSet.LeakDetectionOptions.NotInstalled;

            if (ens.IsEnsembleAvail && ens.IsAncillaryAvail)
            {
                dateAndTime = ens.EnsembleData.EnsDateTime;
                hundredthSec = (byte)ens.EnsembleData.HSec;
                salinity = ens.AncillaryData.Salinity;
                temperature = ens.AncillaryData.WaterTemp;
                depthOfTransducer = ens.AncillaryData.TransducerDepth;
                speedOfSound = ens.AncillaryData.SpeedOfSound;
                BIT = ens.EnsembleData.Status.Value;
            }
            else if(ens.IsBottomTrackAvail)
            {
                salinity = ens.BottomTrackData.Salinity;
                temperature = ens.BottomTrackData.WaterTemp;
                depthOfTransducer = ens.BottomTrackData.TransducerDepth;
                speedOfSound = ens.BottomTrackData.SpeedOfSound;
                BIT = ens.BottomTrackData.Status.Value;
            }

            return new TS(dateAndTime, hundredthSec, salinity, temperature, depthOfTransducer, speedOfSound, BIT, leakDetection).Encode();
        }

        /// <summary>
        /// Get the data out of the ensemble to encode the RA message.
        /// </summary>
        /// <param name="ens">Ensemble data.</param>
        /// <returns>RA string.</returns>
        private string EncodeRA(DataSet.Ensemble ens)
        {
            float pressure = 0.0f;
            float depthB0 = 0.0f;
            float depthB1 = 0.0f;
            float depthB2 = 0.0f;
            float depthB3 = 0.0f;

            if (ens.IsBottomTrackAvail)
            {
                pressure = ens.BottomTrackData.Pressure * 0.0001f;
                depthB0 = ens.BottomTrackData.Range[0];
                depthB1 = ens.BottomTrackData.Range[1];
                depthB2 = ens.BottomTrackData.Range[2];
                depthB3 = ens.BottomTrackData.Range[3];
            }
            else if (ens.IsDvlDataAvail)
            {
                pressure = ens.DvlData.Pressure;
                depthB0 = ens.DvlData.RangeBeam0;
                depthB1 = ens.DvlData.RangeBeam1;
                depthB2 = ens.DvlData.RangeBeam2;
                depthB3 = ens.DvlData.RangeBeam3;
            }

            return new RA(pressure, depthB0, depthB1, depthB2, depthB3).Encode();
        }

        /// <summary>
        /// Encode the WI object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WI string.</returns>
        private string EncodeWI(DataSet.Ensemble ens)
        {
            float x = PD0.BAD_VELOCITY;
            float y = PD0.BAD_VELOCITY;
            float z = PD0.BAD_VELOCITY;
            float q = PD0.BAD_VELOCITY;
            bool isGood = false;
            if(ens.IsInstrumentWaterMassAvail)
            {
                x = ens.InstrumentWaterMassData.VelocityX * 1000.0f;      // convert to mm/s
                y = ens.InstrumentWaterMassData.VelocityY * 1000.0f;
                z = ens.InstrumentWaterMassData.VelocityZ * 1000.0f;
                q = ens.InstrumentWaterMassData.VelocityQ * 1000.0f;

                if( x == DataSet.Ensemble.BAD_VELOCITY || y == DataSet.Ensemble.BAD_VELOCITY || z == DataSet.Ensemble.BAD_VELOCITY || q == DataSet.Ensemble.BAD_VELOCITY )
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new WI(x, y, z, q, isGood).Encode();
            }

            return "";
        }

        /// <summary>
        /// Encode the WS object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WS string.</returns>
        private string EncodeWS(DataSet.Ensemble ens)
        {
            float trans = PD0.BAD_VELOCITY;
            float lon = PD0.BAD_VELOCITY;
            float norm = PD0.BAD_VELOCITY;
            bool isGood = false;
            if (ens.IsShipWaterMassAvail)
            {
                trans = ens.ShipWaterMassData.VelocityTransverse * 1000.0f;      // convert to mm/s
                lon = ens.ShipWaterMassData.VelocityLongitudinal * 1000.0f;
                norm = ens.ShipWaterMassData.VelocityNormal * 1000.0f;

                if (trans == DataSet.Ensemble.BAD_VELOCITY || lon == DataSet.Ensemble.BAD_VELOCITY || norm == DataSet.Ensemble.BAD_VELOCITY)
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new WS(trans, lon, norm, isGood).Encode();
            }

            return "";
        }

        /// <summary>
        /// Encode the WE object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WI string.</returns>
        private string EncodeWE(DataSet.Ensemble ens)
        {
            float east = PD0.BAD_VELOCITY;
            float north = PD0.BAD_VELOCITY;
            float vert = PD0.BAD_VELOCITY;
            bool isGood = false;
            if (ens.IsInstrumentWaterMassAvail)
            {
                east = ens.EarthWaterMassData.VelocityEast * 1000.0f;      // convert to mm/s
                north = ens.EarthWaterMassData.VelocityNorth * 1000.0f;
                vert = ens.EarthWaterMassData.VelocityVertical * 1000.0f;

                if (east == DataSet.Ensemble.BAD_VELOCITY || north == DataSet.Ensemble.BAD_VELOCITY || vert == DataSet.Ensemble.BAD_VELOCITY)
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new WE(east, north, vert, isGood).Encode();
            }

            return "";
        }

        /// <summary>
        /// Encode the BI object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WI string.</returns>
        private string EncodeBI(DataSet.Ensemble ens)
        {
            float x = PD0.BAD_VELOCITY;
            float y = PD0.BAD_VELOCITY;
            float z = PD0.BAD_VELOCITY;
            float q = PD0.BAD_VELOCITY;
            bool isGood = false;
            if (ens.IsBottomTrackAvail)
            {
                x = ens.BottomTrackData.InstrumentVelocity[0] * 1000.0f;      // convert to mm/s
                y = ens.BottomTrackData.InstrumentVelocity[1] * 1000.0f;
                z = ens.BottomTrackData.InstrumentVelocity[2] * 1000.0f;
                q = ens.BottomTrackData.InstrumentVelocity[3] * 1000.0f;

                if (x == DataSet.Ensemble.BAD_VELOCITY || y == DataSet.Ensemble.BAD_VELOCITY || z == DataSet.Ensemble.BAD_VELOCITY || q == DataSet.Ensemble.BAD_VELOCITY)
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new BI(x, y, z, q, isGood).Encode();
            }

            return "";
        }

        /// <summary>
        /// Encode the BS object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WI string.</returns>
        private string EncodeBS(DataSet.Ensemble ens)
        {
            float trans = PD0.BAD_VELOCITY;
            float lon = PD0.BAD_VELOCITY;
            float up = PD0.BAD_VELOCITY;
            bool isGood = false;
            if (ens.IsBottomTrackAvail)
            {
                trans = ens.BottomTrackData.ShipVelocity[0] * 1000.0f;      // convert to mm/s
                lon = ens.BottomTrackData.ShipVelocity[1] * 1000.0f;
                up = ens.BottomTrackData.ShipVelocity[2] * 1000.0f;

                if (trans == DataSet.Ensemble.BAD_VELOCITY || lon == DataSet.Ensemble.BAD_VELOCITY || up == DataSet.Ensemble.BAD_VELOCITY)
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new BS(trans, lon, up, isGood).Encode();
            }

            return "";
        }

        /// <summary>
        /// Encode the BE object.
        /// </summary>
        /// <param name="ens">Ensemble.</param>
        /// <returns>WI string.</returns>
        private string EncodeBE(DataSet.Ensemble ens)
        {
            float east = PD0.BAD_VELOCITY;
            float north = PD0.BAD_VELOCITY;
            float vert = PD0.BAD_VELOCITY;
            bool isGood = false;
            if (ens.IsBottomTrackAvail)
            {
                east = ens.BottomTrackData.EarthVelocity[0] * 1000.0f;      // convert to mm/s
                north = ens.BottomTrackData.EarthVelocity[1] * 1000.0f;
                vert = ens.BottomTrackData.EarthVelocity[2] * 1000.0f;

                if (east == DataSet.Ensemble.BAD_VELOCITY || north == DataSet.Ensemble.BAD_VELOCITY || vert == DataSet.Ensemble.BAD_VELOCITY)
                {
                    isGood = false;
                }
                else
                {
                    isGood = true;
                }

                return new BE(east, north, vert, isGood).Encode();
            }

            return "";
        }
    }
}
