﻿//  Copyright 2021 Google LLC. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NtApiDotNet.Net.Firewall
{
    /// <summary>
    /// Static class for firewall utility functions.
    /// </summary>
    public static class FirewallUtils
    {
        #region Public Members
        /// <summary>
        /// Name for fake NT type.
        /// </summary>
        public const string FIREWALL_NT_TYPE_NAME = "Firewall";

        /// <summary>
        /// Name for fake filter NT type.
        /// </summary>
        public const string FIREWALL_FILTER_NT_TYPE_NAME = "FirewallFilter";

        /// <summary>
        /// Get the NT type for the firewall.
        /// </summary>
        public static NtType FirewallType => NtType.GetTypeByName(FIREWALL_NT_TYPE_NAME);

        /// <summary>
        /// Get the NT type for the firewall.
        /// </summary>
        public static NtType FirewallFilterType => NtType.GetTypeByName(FIREWALL_FILTER_NT_TYPE_NAME);

        /// <summary>
        /// Get the generic mapping for a firewall object.
        /// </summary>
        /// <returns>The firewall object generic mapping.</returns>
        public static GenericMapping GetGenericMapping()
        {
            return new GenericMapping()
            {
                GenericRead = FirewallAccessRights.ReadControl | FirewallAccessRights.BeginReadTxn |
                        FirewallAccessRights.Classify | FirewallAccessRights.Open |
                        FirewallAccessRights.Read | FirewallAccessRights.ReadStats,
                GenericExecute = FirewallAccessRights.ReadControl | FirewallAccessRights.Enum |
                        FirewallAccessRights.Subscribe,
                GenericWrite = FirewallAccessRights.ReadControl | FirewallAccessRights.Add |
                        FirewallAccessRights.AddLink | FirewallAccessRights.BeginWriteTxn |
                        FirewallAccessRights.Write,
                GenericAll = FirewallAccessRights.Delete | FirewallAccessRights.WriteDac |
                        FirewallAccessRights.WriteOwner | FirewallAccessRights.ReadControl |
                        FirewallAccessRights.BeginReadTxn | FirewallAccessRights.Classify |
                        FirewallAccessRights.Open | FirewallAccessRights.Read |
                        FirewallAccessRights.ReadStats | FirewallAccessRights.Enum |
                        FirewallAccessRights.Subscribe | FirewallAccessRights.Add |
                        FirewallAccessRights.AddLink | FirewallAccessRights.BeginWriteTxn |
                        FirewallAccessRights.Write
            };
        }

        /// <summary>
        /// Get the generic mapping for a firewall filter object.
        /// </summary>
        /// <returns>The firewall filter object generic mapping.</returns>
        public static GenericMapping GetFilterGenericMapping()
        {
            return new GenericMapping()
            {
                GenericRead = FirewallFilterAccessRights.ReadControl,
                GenericExecute = FirewallFilterAccessRights.ReadControl | FirewallFilterAccessRights.Match,
                GenericWrite = FirewallFilterAccessRights.ReadControl,
                GenericAll = FirewallFilterAccessRights.ReadControl | FirewallFilterAccessRights.Match
            };
        }

        /// <summary>
        /// Get App ID from a filename.
        /// </summary>
        /// <param name="filename">The filename to convert.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The App ID.</returns>
        public static NtResult<string> GetAppIdFromFileName(string filename, bool throw_on_error)
        {
            return FirewallNativeMethods.FwpmGetAppIdFromFileName0(filename, out SafeFwpmMemoryBuffer appid).CreateWin32Result(
                throw_on_error, () => {
                using (appid)
                {
                    appid.Initialize<FWP_BYTE_BLOB>(1);
                    var blob = appid.Read<FWP_BYTE_BLOB>(0);
                    return Encoding.Unicode.GetString(blob.ToArray()).TrimEnd('\0');
                }
            });
        }

        /// <summary>
        /// Get App ID from a filename.
        /// </summary>
        /// <param name="filename">The filename to convert.</param>
        /// <returns>The App ID.</returns>
        public static string GetAppIdFromFileName(string filename)
        {
            return GetAppIdFromFileName(filename, true).Result;
        }

        /// <summary>
        /// Get a list of known layer names.
        /// </summary>
        /// <returns>The list of known layer names.</returns>
        public static IEnumerable<string> GetKnownLayerNames()
        {
            return NamedGuidDictionary.LayerGuids.Value.Values;
        }

        /// <summary>
        /// Get a list of known layer guids.
        /// </summary>
        /// <returns>The list of known layer guids.</returns>
        public static IEnumerable<Guid> GetKnownLayerGuids()
        {
            return NamedGuidDictionary.LayerGuids.Value.Keys;
        }

        /// <summary>
        /// Get a known layer GUID from its name.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <returns>The known layer GUID.</returns>
        public static Guid GetKnownLayerGuid(string name)
        {
            return NamedGuidDictionary.LayerGuids.Value.GuidFromName(name);
        }

        /// <summary>
        /// Get a list of known sub-layer names.
        /// </summary>
        /// <returns>The list of known sub-layer names.</returns>
        public static IEnumerable<string> GetKnownSubLayerNames()
        {
            return NamedGuidDictionary.SubLayerGuids.Value.Values;
        }

        /// <summary>
        /// Get a list of known sub-layer guids.
        /// </summary>
        /// <returns>The list of known sub-layer guids.</returns>
        public static IEnumerable<Guid> GetKnownSubLayerGuids()
        {
            return NamedGuidDictionary.SubLayerGuids.Value.Keys;
        }

        /// <summary>
        /// Get a known sub-layer GUID from its name.
        /// </summary>
        /// <param name="name">The name of the sub-layer.</param>
        /// <returns>The known sub-layer GUID.</returns>
        public static Guid GetKnownSubLayerGuid(string name)
        {
            return NamedGuidDictionary.SubLayerGuids.Value.GuidFromName(name);
        }

        /// <summary>
        /// Get a layer GUID for an ALE layer enumeration.
        /// </summary>
        /// <param name="ale_layer">The ALE layer enumeration.</param>
        /// <returns>The ALE layer GUID.</returns>
        public static Guid GetLayerGuidForAleLayer(FirewallAleLayer ale_layer)
        {
            switch (ale_layer)
            {
                case FirewallAleLayer.ConnectV4:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_CONNECT_V4;
                case FirewallAleLayer.ConnectV6:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_CONNECT_V6;
                case FirewallAleLayer.ListenV4:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_LISTEN_V4;
                case FirewallAleLayer.ListenV6:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_LISTEN_V6;
                case FirewallAleLayer.RecvAcceptV4:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V4;
                case FirewallAleLayer.RecvAcceptV6:
                    return FirewallLayerGuids.FWPM_LAYER_ALE_AUTH_RECV_ACCEPT_V6;
                default:
                    throw new ArgumentException("Unknown ALE layer", nameof(ale_layer));
            }
        }

        #endregion

        #region Internal Methods
        internal static Guid? ReadGuid(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
        }

        internal static T CloneValue<T>(this T value)
        {
            if (value is ICloneable clone)
                return (T)clone.Clone();
            return value;
        }

        #endregion
    }
}
