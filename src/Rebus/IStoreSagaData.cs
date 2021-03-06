﻿using System;

namespace Rebus
{
    /// <summary>
    /// Implement this in order to control how saga data gets persisted
    /// </summary>
    public interface IStoreSagaData
    {
        void Save(ISagaData sagaData, string[] sagaDataPropertyPathsToIndex);
        void Delete(ISagaData sagaData);
        ISagaData Find(string sagaDataPropertyPath, string fieldFromMessage, Type sagaDataType);
    }
}