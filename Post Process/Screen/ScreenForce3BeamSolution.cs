// -----------------------------------------------------------------------
// <copyright file="ScreenForce3BeamSolution.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace RTI
{
    namespace ScreenData
    {
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Text;

        /// <summary>
        /// TODO: Update summary.
        /// </summary>
        public class ScreenForce3BeamSolution
        {

            /// <summary>
            /// Force a specific beam to be bad to do a 3 beam solution.  This will take the beam
            /// from the options to choose the beam to mark bad.  It will then mark the Beam Velocity value
            /// bad for all the bins in the Beam Velocity.  It will then recalculate Earth Velocity data based
            /// off the new Beam Velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to set bad beam.</param>
            /// <param name="beam">Beam to mark bad.</param>
            /// <returns>TRUE = Screening could be done.</returns>
            /// <param name="origDataFormat">Original Data format.</param>
            public static bool Force3BeamSolution(ref DataSet.Ensemble ensemble, int beam, AdcpCodec.CodecEnum origDataFormat)
            {
                // Verify the data exist
                if (!ensemble.IsEnsembleAvail || !ensemble.IsBeamVelocityAvail || !ensemble.IsGoodBeamAvail)
                {
                    return false;
                }

                // Verify the given bad beam exist
                if (beam < 0 || beam > ensemble.EnsembleData.NumBeams)
                {
                    return false;
                }

                // Set all the velocities to bad for the given bad beam
                for (int bin = 0; bin < ensemble.BeamVelocityData.NumElements; bin++)
                {
                    ensemble.BeamVelocityData.BeamVelocityData[bin, beam] = DataSet.Ensemble.BAD_VELOCITY;
                    ensemble.GoodBeamData.GoodBeamData[bin, beam] = 0;
                }

                // Calculate the new Earth velocities
                Transform.ProfileTransform(ref ensemble, origDataFormat);

                return true;
            }

            /// <summary>
            /// Force a specific beam to be bad to do a 3 beam solution.  This will take the beam
            /// from the options to choose the beam to mark bad.  It will then mark the Bottom Track Beam Velocity value
            /// bad for all the bins in the Bottom Track Beam Velocity.  It will then recalculate Bottom Track Earth Velocity data based
            /// off the new Bottom Track Beam Velocity.
            /// </summary>
            /// <param name="ensemble">Ensemble to set bad beam.</param>
            /// <param name="beam">Beam to mark bad.</param>
            /// <param name="origDataFormat">Original Data Format.</param>
            /// <returns>TRUE = Screening could be done.</returns>
            public static bool Force3BottomTrackBeamSolution(ref DataSet.Ensemble ensemble, int beam, AdcpCodec.CodecEnum origDataFormat)
            {
                // Verify the data exist
                if (!ensemble.IsEnsembleAvail || !ensemble.IsBottomTrackAvail)
                {
                    return false;
                }

                // Verify the given bad beam exist
                if (beam < 0 || beam > ensemble.BottomTrackData.NumBeams)
                {
                    return false;
                }

                // Set all the velocities to bad for the given bad beam
                ensemble.BottomTrackData.BeamVelocity[beam] = DataSet.Ensemble.BAD_VELOCITY;
                ensemble.BottomTrackData.BeamGood[beam] = 0;
               
                // Calculate the new Earth velocities
                Transform.BottomTrackTransform(ref ensemble, origDataFormat);

                return true;
            }

        }
    }
}
