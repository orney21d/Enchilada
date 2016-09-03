﻿namespace Enchilada.Filesystem
{
    using System;
    using System.Linq;
    using Configuration;
    using Infrastructure.Exceptions;
    using Infrastructure.Interface;

    public class EnchiladaFileProviderResolver : IEnchiladaFilesystemResolver
    {
        private readonly EnchiladaConfiguration enchiladaConfiguration;

        public EnchiladaFileProviderResolver( EnchiladaConfiguration enchiladaConfiguration = null )
        {
            this.enchiladaConfiguration = enchiladaConfiguration;
        }

        protected IFileProvider OpenProvider( string uri )
        {
            if ( enchiladaConfiguration == null || !enchiladaConfiguration.Adapters.Any() )
                throw new ConfigurationMissingException();

            var providerUri = new Uri( uri );

            var matchingProvider = enchiladaConfiguration.Adapters
                                                         .FirstOrDefault( x => x.AdapterName == providerUri.Host );

            return (IFileProvider) Activator.CreateInstance( matchingProvider.FileProvider, matchingProvider, providerUri.PathAndQuery );
        }

        public IDirectory OpenDirectory( string uri )
        {
            var provider = OpenProvider( uri );

            return provider.RootDirectory;
        }

        public IFile OpenFile( string uri )
        {
            var provider = OpenProvider( uri );

            return provider.File;
        }
    }
}