﻿//  Copyright 2019 Google Inc. All Rights Reserved.
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

using System;
using System.Runtime.InteropServices;

namespace NtApiDotNet
{
    /// <summary>
    /// Access rights for ALPC
    /// </summary>
#pragma warning disable 1591
    [Flags]
    public enum AlpcAccessRights : uint
    {
        Connect = 0x1,
        GenericRead = GenericAccessRights.GenericRead,
        GenericWrite = GenericAccessRights.GenericWrite,
        GenericExecute = GenericAccessRights.GenericExecute,
        GenericAll = GenericAccessRights.GenericAll,
        Delete = GenericAccessRights.Delete,
        ReadControl = GenericAccessRights.ReadControl,
        WriteDac = GenericAccessRights.WriteDac,
        WriteOwner = GenericAccessRights.WriteOwner,
        Synchronize = GenericAccessRights.Synchronize,
        MaximumAllowed = GenericAccessRights.MaximumAllowed,
        AccessSystemSecurity = GenericAccessRights.AccessSystemSecurity
    }

    /// <summary>
    /// ALPC Port Information Class
    /// </summary>
    public enum AlpcPortInformationClass
    {
        AlpcBasicInformation,
        AlpcPortInformation,
        AlpcAssociateCompletionPortInformation,
        AlpcConnectedSIDInformation,
        AlpcServerInformation,
        AlpcMessageZoneInformation,
        AlpcRegisterCompletionListInformation,
        AlpcUnregisterCompletionListInformation,
        AlpcAdjustCompletionListConcurrencyCountInformation,
        AlpcRegisterCallbackInformation,
        AlpcCompletionListRundownInformation,
        AlpcWaitForPortReferences
    }

    public enum AlpcMessageInformationClass
    {
        AlpcMessageSidInformation = 0,
        AlpcMessageTokenModifiedIdInformation,
        AlpcMessageDirectStatusInformation,
        AlpcMessageHandleInformation,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AlpcPortMessage
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct PortMessageUnion1
        {
            [FieldOffset(0)]
            public short DataLength;
            [FieldOffset(2)]
            public short TotalLength;
            [FieldOffset(0)]
            public int Length;
        }
        public PortMessageUnion1 u1;

        [StructLayout(LayoutKind.Explicit)]
        public struct PortMessageUnion2
        {
            [FieldOffset(0)]
            public short Type;
            [FieldOffset(2)]
            public short DataInfoOffset;
            [FieldOffset(0)]
            public int ZeroInit;
        }
        public PortMessageUnion2 u2;

        public ClientIdStruct ClientId;
        public int MessageId;

        [StructLayout(LayoutKind.Explicit)]
        public struct PortMessageUnion3
        {
            [FieldOffset(0)]
            public IntPtr ClientViewSize;
            [FieldOffset(0)]
            public int CallbackId;
        }
        public PortMessageUnion3 u3;
    }

    [Flags]
    public enum AlpcPortAttributeFlags
    {
        None = 0,
        LpcPort = 0x1000, // Not accessible outside the kernel.
        AllowImpersonation = 0x10000,
        AllowLpcRequests = 0x20000,
        WaitablePort = 0x40000,
        AllowDupObject = 0x80000,
        SystemProcess = 0x100000, // Not accessible outside the kernel.
        LrpcWakePolicy1 = 0x200000,
        LrpcWakePolicy2 = 0x400000,
        LrpcWakePolicy3 = 0x800000,
        Unknown1000000 = 0x1000000,
        /// <summary>
        /// If set then object duplication won't complete. Used by RPC to ensure
        /// multi-handle attributes don't fail when receiving.
        /// </summary>
        NoCompleteDupObject = 0x2000000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AlpcPortAttributes
    {
        public AlpcPortAttributeFlags Flags;
        public SecurityQualityOfServiceStruct SecurityQos;
        public IntPtr MaxMessageLength;
        public IntPtr MemoryBandwidth;
        public IntPtr MaxPoolUsage;
        public IntPtr MaxSectionSize;
        public IntPtr MaxViewSize;
        public IntPtr MaxTotalSectionSize;
        public AlpcHandleObjectType DupObjectTypes;
        public int Reserved;

        public static AlpcPortAttributes CreateDefault()
        {
            return new AlpcPortAttributes()
            {
                Flags = AlpcPortAttributeFlags.None,
                SecurityQos = new SecurityQualityOfServiceStruct(SecurityImpersonationLevel.Impersonation,
                                                            SecurityContextTrackingMode.Static, false),
                MaxMessageLength = new IntPtr(short.MaxValue),
                DupObjectTypes = AlpcHandleObjectType.AllObjects
            };
        }
    }

    [Flags]
    public enum AlpcSecurityAttributeFlags
    {
        None = 0,
        CreateHandle = 0x20000,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcHandle
    {
        private IntPtr _value;

        public long Value
        {
            get => _value.ToInt64();
            set => _value = new IntPtr(value);
        }

        public AlpcHandle(long value)
        {
            _value = new IntPtr(value);
        }
        
        public static implicit operator AlpcHandle(long value)
        {
            return new AlpcHandle(value);
        }
    }

    [Flags]
    public enum AlpcMessageAttributeFlags : uint
    {
        None = 0,
        WorkOnBehalfOf = 0x2000000,
        Direct = 0x4000000,
        Token = 0x8000000,
        Handle = 0x10000000,
        Context = 0x20000000,
        View = 0x40000000,
        Security = 0x80000000,
    }

    public enum AlpcSecurityAttrFlags
    {
        None = 0,
        CreateHandle = 0x20000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcSecurityAttr
    {
        public AlpcSecurityAttrFlags Flags;
        public IntPtr QoS; // struct _SECURITY_QUALITY_OF_SERVICE
        public AlpcHandle ContextHandle;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcContextAttr
    {
        public IntPtr PortContext;
        public IntPtr MessageContext;
        public int Sequence;
        public int MessageId;
        public int CallbackId;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcDirectAttr
    {
        public IntPtr Event;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcWorkOnBehalfTicket
    {
        public int ThreadId;
        public int ThreadCreationTimeLow;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcWorkOnBehalfAttr
    {
        public AlpcWorkOnBehalfTicket Ticket;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcTokenAttr
    {
        public Luid TokenId;
        public Luid AuthenticationId;
        public Luid ModifiedId;
    }

    [Flags]
    public enum AlpcHandleAttrFlags
    {
        None = 0,
        SameAccess = 0x10000,
        SameAttributes = 0x20000,
        Indirect = 0x40000,
        Inherit = 0x80000
    }

    [Flags]
    public enum AlpcHandleObjectType
    {
        None = 0,
        File = 0x0001,
        Unknown0002 = 0x0002,
        Thread = 0x0004,
        Semaphore = 0x0008,
        Event = 0x0010,
        Process = 0x0020,
        Mutex = 0x0040,
        Section = 0x0080,
        RegKey = 0x0100,
        Token = 0x0200,
        Composition = 0x0400,
        Job = 0x0800,
        AllObjects = 0xFFD,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcHandleAttr
    {
        public AlpcHandleAttrFlags Flags;
        public IntPtr Handle; // Also ALPC_HANDLE_ATTR32* HandleAttrArray;
        public AlpcHandleObjectType ObjectType; // Also HandleCount;
        public AccessMask DesiredAccess; // Also GrantedAccess
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcHandleAttr32
    {
        public AlpcHandleAttrFlags Flags;
        public int Handle;
        public AlpcHandleObjectType ObjectType;
        public AccessMask DesiredAccess;
    }

    [Flags]
    public enum AlpcDataViewAttrFlags
    {
        None = 0,
        NotSecure = 0x40000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcDataViewAttr
    {
        public AlpcDataViewAttrFlags Flags;
        public AlpcHandle SectionHandle;
        public IntPtr ViewBase;
        public long ViewSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcMessageAttributes
    {
        public AlpcMessageAttributeFlags AllocatedAttributes;
        public AlpcMessageAttributeFlags ValidAttributes;
    }

    [Flags]
    public enum AlpcDisconnectPortFlags
    {
        None = 0,
        NoFlushOnClose = 1,
    }

    [Flags]
    public enum AlpcMessageFlags : uint
    {
        None = 0,
        ReplyMessage = 0x1,
        LpcMode = 0x2,
        ReleaseMessage = 0x10000,
        SyncRequest = 0x20000,
        WaitUserMode = 0x100000,
        WaitAlertable = 0x200000,
        Wow64Call = 0x80000000
    }

    [Flags]
    public enum AlpcCancelMessageFlags
    {
        None = 0,
        TryCancel = 1,
        Unknown2 = 2,
        Unknown4 = 4,
        NoContextCheck = 8
    }

    [Flags]
    public enum AlpcImpersonationFlags
    {
        None = 0,
        AnonymousFallback = 1,
        RequireImpersonationLevel = 2,
        // From bit 2 on it's the impersonation level required.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcBasicInformation
    {
        public int Flags;
        public int SequenceNo;
        public IntPtr PortContext;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcPortAssociateCompletionPort
    {
        public IntPtr CompletionKey;
        public SafeKernelObjectHandle CompletionPort;
    }

    public struct AlpcServerInformationOut
    {
        public bool ThreadBlocked;
        public IntPtr ConnectedProcessId;
        public UnicodeStringOut ConnectionPortName;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct AlpcServerInformation
    {
        [FieldOffset(0)]
        public IntPtr ThreadHandle;
        [FieldOffset(0)]
        public AlpcServerInformationOut Out;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcPortMessageZoneInformation
    {
        public IntPtr Buffer;
        public int Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AlpcPortCompletionListInformation
    {
        public IntPtr Buffer; // PALPC_COMPLETION_LIST_HEADER
        public int Size;
        public int ConcurrencyCount;
        public int AttributeFlags;
    }

    [Flags]
    public enum AlpcOpenSenderProcessFlags
    {
        None = 0,
    }

    [Flags]
    public enum AlpcOpenSenderThreadFlags
    {
        None = 0,
    }

    public static class NtAlpcNativeMethods
    {
        [DllImport("ntdll.dll")]
        public static extern int AlpcMaxAllowedMessageLength();

        [DllImport("ntdll.dll")]
        public static extern int AlpcGetHeaderSize(int Flags);

        [DllImport("ntdll.dll")]
        public static extern NtStatus AlpcInitializeMessageAttribute(
            AlpcMessageAttributeFlags AttributeFlags,
            SafeAlpcMessageAttributesBuffer Buffer,
            int BufferSize,
            out int RequiredBufferSize
        );

        [DllImport("ntdll.dll")]
        public static extern IntPtr AlpcGetMessageAttribute(
            SafeAlpcMessageAttributesBuffer Buffer,
            AlpcMessageAttributeFlags AttributeFlag
        );
    }

    public static partial class NtSystemCalls
    {
        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCreatePort(
            out SafeKernelObjectHandle PortHandle,
            [In] ObjectAttributes ObjectAttributes,
            [In] AlpcPortAttributes PortAttributes
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcDisconnectPort(
            [In] SafeKernelObjectHandle PortHandle,
            AlpcDisconnectPortFlags Flags
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcQueryInformation(
            SafeKernelObjectHandle PortHandle,
            AlpcPortInformationClass PortInformationClass,
            SafeBuffer PortInformation,
            int Length,
            out int ReturnLength
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcSetInformation(
            [In] SafeKernelObjectHandle PortHandle,
            AlpcPortInformationClass PortInformationClass,
            SafeBuffer PortInformation,
            int Length);

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcConnectPort(
            out SafeKernelObjectHandle PortHandle,
            [In] UnicodeString PortName,
            [In] ObjectAttributes ObjectAttributes,
            [In] AlpcPortAttributes PortAttributes,
            AlpcMessageFlags Flags,
            [In] SafeSidBufferHandle RequiredServerSid,
            [In, Out] SafeAlpcPortMessageBuffer ConnectionMessage,
            [In, Out] OptionalLength BufferLength,
            [In, Out] SafeAlpcMessageAttributesBuffer OutMessageAttributes,
            [In, Out] SafeAlpcMessageAttributesBuffer InMessageAttributes,
            [In] LargeInteger Timeout
        );

        [DllImport("ntdll.dll")]
        [SupportedVersion(SupportedVersion.Windows8)]
        public static extern NtStatus NtAlpcConnectPortEx(
            out SafeKernelObjectHandle PortHandle,
            [In] ObjectAttributes ConnectionPortObjectAttributes,
            [In] ObjectAttributes ClientPortObjectAttributes,
            [In] AlpcPortAttributes PortAttributes,
            AlpcMessageFlags Flags,
            [In] SafeBuffer ServerSecurityRequirements, // SECURITY_DESCRIPTOR
            [In, Out] SafeAlpcPortMessageBuffer ConnectionMessage,
            [In, Out] OptionalLength BufferLength,
            [In, Out] SafeAlpcMessageAttributesBuffer OutMessageAttributes,
            [In, Out] SafeAlpcMessageAttributesBuffer InMessageAttributes,
            [In] LargeInteger Timeout);

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcSendWaitReceivePort(
            [In] SafeKernelObjectHandle PortHandle,
            AlpcMessageFlags Flags,
            [In] SafeAlpcPortMessageBuffer SendMessage,
            [In, Out] SafeAlpcMessageAttributesBuffer SendMessageAttributes,
            [Out] SafeAlpcPortMessageBuffer ReceiveMessage,
            [In, Out] OptionalLength BufferLength,
            [In, Out] SafeAlpcMessageAttributesBuffer ReceiveMessageAttributes,
            [In] LargeInteger Timeout);

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCancelMessage(
            [In] SafeKernelObjectHandle PortHandle,
            uint Flags,
            ref AlpcContextAttr MessageContext
           );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcImpersonateClientOfPort(
                [In] SafeKernelObjectHandle PortHandle,
                [In] SafeAlpcPortMessageBuffer PortMessage,
                AlpcImpersonationFlags Flags
        );

        [DllImport("ntdll.dll")]
        [SupportedVersion(SupportedVersion.Windows10_TH2)]
        public static extern NtStatus NtAlpcImpersonateClientContainerOfPort(
            [In] SafeKernelObjectHandle PortHandle,
            [In] SafeAlpcPortMessageBuffer PortMessage,
            int Flags
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCreateSecurityContext(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            ref AlpcSecurityAttr SecurityAttribute);

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcDeleteSecurityContext(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            AlpcHandle ContextHandle
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcRevokeSecurityContext(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            AlpcHandle ContextHandle
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcQueryInformationMessage(
            SafeKernelObjectHandle PortHandle,
            AlpcPortMessage PortMessage,
            AlpcMessageInformationClass MessageInformationClass,
            SafeBuffer MessageInformation,
            int Length,
            out int ReturnLength
        );

        // Version to support AlpcMessageDirectStatusInformation which needs ReturnLength == NULL.
        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcQueryInformationMessage(
            SafeKernelObjectHandle PortHandle,
            AlpcPortMessage PortMessage,
            AlpcMessageInformationClass MessageInformationClass,
            IntPtr MessageInformation,
            int Length,
            IntPtr ReturnLength
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCreatePortSection(
            SafeKernelObjectHandle PortHandle,
            AlpcDataViewAttrFlags Flags,
            SafeKernelObjectHandle SectionHandle,
            IntPtr SectionSize,
            out AlpcHandle AlpcSectionHandle,
            out IntPtr ActualSectionSize
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcDeletePortSection(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            AlpcHandle SectionHandle
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCreateResourceReserve(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            IntPtr MessageSize,
            out AlpcHandle ResourceId
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcDeleteResourceReserve(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            AlpcHandle ResourceId
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCreateSectionView(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            ref AlpcDataViewAttr ViewAttributes
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcDeleteSectionView(
            SafeKernelObjectHandle PortHandle,
            int Flags,
            IntPtr ViewBase
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcAcceptConnectPort(
            out SafeKernelObjectHandle PortHandle,
            SafeKernelObjectHandle ConnectionPortHandle,
            AlpcMessageFlags Flags,
            ObjectAttributes ObjectAttributes,
            AlpcPortAttributes PortAttributes,
            IntPtr PortContext,
            AlpcPortMessage ConnectionRequest,
            SafeAlpcMessageAttributesBuffer ConnectionMessageAttributes,
            bool AcceptConnection
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcOpenSenderProcess(
            out SafeKernelObjectHandle ProcessHandle,
            SafeKernelObjectHandle PortHandle,
            AlpcPortMessage PortMessage,
            AlpcOpenSenderProcessFlags Flags,
            ProcessAccessRights DesiredAccess,
            ObjectAttributes ObjectAttributes
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcOpenSenderThread(
            out SafeKernelObjectHandle ThreadHandle,
            SafeKernelObjectHandle PortHandle,
            AlpcPortMessage PortMessage,
            AlpcOpenSenderThreadFlags Flags,
            ThreadAccessRights DesiredAccess,
            ObjectAttributes ObjectAttributes
        );

        [DllImport("ntdll.dll")]
        public static extern NtStatus NtAlpcCancelMessage(
            SafeKernelObjectHandle PortHandle,
            AlpcCancelMessageFlags Flags,
            ref AlpcContextAttr MessageContext
        );
    }
#pragma warning restore 1591
}
