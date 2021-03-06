﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

using WcfService.CertificateResources;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class TcpTransportSecuritySslCustomCertValidationResource : TcpResource
    {
        protected override string Address { get { return "tcp-server-ssl-security-clientcredentialtype-certificate-customvalidator"; } }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            string thumbprint = CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration);

            serviceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            serviceHost.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new MyX509CertificateValidator("DO_NOT_TRUST_WcfBridgeRootCA");
            serviceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                      StoreName.My,
                                                      X509FindType.FindByThumbprint,
                                                      thumbprint);
        }
    }
}
