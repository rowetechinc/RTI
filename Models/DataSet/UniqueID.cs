/*
 * Copyright © 2011 
 * Rowe Technology Inc.
 * All rights reserved.
 * http://www.rowetechinc.com
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification is NOT permitted.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 * 
 * HISTORY
 * -----------------------------------------------------------------
 * Date            Initials    Version    Comments
 * -----------------------------------------------------------------
 * 12/16/2011      RC          1.10       Initial coding.
 *       
 */


using System;
namespace RTI
{

    namespace DataSet
    {
        /// <summary>
        /// This will create a uniqueId for the
        /// dataset within the database.  Because
        /// multiple recordings could be added to
        /// one database, ensemble numbers may duplicate.
        /// By using the DateTime and Ensemble number
        /// we can ensure we are getting a specific
        /// dataset.
        /// </summary>
        public class UniqueID
        {
            /// <summary>
            /// Ensemble number.
            /// </summary>
            public int EnsembleNumber { get; set; }

            /// <summary>
            /// Ensemble date and time.
            /// </summary>
            public DateTime EnsDateTime { get; set; }

            /// <summary>
            /// Ensemble Unique ID.  This is made up of the
            /// date and time and the ensemble number.
            /// </summary>
            /// <param name="ensNum">Ensemble Number.</param>
            /// <param name="ensDateTime">Ensemble date and time.</param>
            public UniqueID(int ensNum, DateTime ensDateTime)
            {
                EnsembleNumber = ensNum;
                EnsDateTime = ensDateTime;
            }

            /// <summary>
            /// Determine if the 2 ids given are the equal.
            /// </summary>
            /// <param name="id1">First ID to check.</param>
            /// <param name="id2">ID to check against.</param>
            /// <returns>True if they are the same.</returns>
            public static bool operator ==(UniqueID id1, UniqueID id2)
            {
                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(id1, id2))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object)id1 == null) || ((object)id2 == null))
                {
                    return false;
                }

                // Return true if the fields match:
                return id1.EnsembleNumber == id2.EnsembleNumber && id1.EnsDateTime == id2.EnsDateTime;
            }

            /// <summary>
            /// Check if the ID are not equal.  Invert Equal.
            /// </summary>
            /// <param name="id1">First ID to check.</param>
            /// <param name="id2">ID to check against.</param>
            /// <returns>True if they are NOT the same.</returns>
            public static bool operator !=(UniqueID id1, UniqueID id2)
            {
                return !(id1 == id2);
            }


            /// <summary>
            /// Check if the id is equal.
            /// </summary>
            /// <param name="obj">Id to check against.</param>
            /// <returns>True if the ID is equal to this object.</returns>
            public override bool Equals(Object obj)
            {
                // If parameter is null return false.
                if (obj == null)
                {
                    return false;
                }

                // If parameter cannot be cast to Point return false.
                UniqueID id = obj as UniqueID;
                if ((System.Object)id == null)
                {
                    return false;
                }

                return this == id;
            }

            /// <summary>
            /// String representation of this object
            /// is the the EnsembleNumber_DateTime.
            /// </summary>
            /// <returns>String representation of this object.</returns>
            public override string ToString()
            {
                return EnsembleNumber.ToString() + "_" + EnsDateTime.ToString();
            }

            /// <summary>
            /// Hash code for this object.
            /// </summary>
            /// <returns>HashCode for this object.</returns>
            public override int GetHashCode()
            {
                return this.EnsDateTime.GetHashCode() + this.EnsembleNumber;
            }
        }
    }
}