﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.CRMMobile.Configuration
{
    public class AppSettings
    {
        public int StorageServerId { get; set; }
        public string AuthorizeEndpointUrlAddress { get; set; }
        public string BaseRootPath { get; set; }
        public string SoapHeaderUsername { get; set; }
        public string SoapHeaderPassword { get; set; }
        public string TresourceFilePath { get; set; }
        public string LogPath { get; set; }
        public string PictureRootURL { get; set; }
        public string ApiKey { get; set; }
        public string ApiToken { get; set; }
        public string AuthorizeURL { get; set; }
        public string ThaiBulkApiKey { get; set; }
        public string ThaiBulkSecret { get; set; }
        public string ThaiBulkVerifyOTPURL { get; set; }
        public string ThaiBulkRequestOTPURL { get; set; }
    }
}
