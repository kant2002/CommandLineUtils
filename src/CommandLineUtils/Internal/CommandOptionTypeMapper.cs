﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace McMaster.Extensions.CommandLineUtils
{
    internal class CommandOptionTypeMapper
    {
        private CommandOptionTypeMapper()
        { }

        public static CommandOptionTypeMapper Default { get; } = new CommandOptionTypeMapper();

        public bool TryGetOptionType(Type clrType, out CommandOptionType optionType)
        {
            try
            {
                optionType = GetOptionType(clrType);
                return true;
            }
            catch
            {
                optionType = default;
                return false;
            }
        }

        public CommandOptionType GetOptionType(Type clrType)
        {
            if (clrType == typeof(bool))
            {
                return CommandOptionType.NoValue;
            }

            if (clrType == typeof(string))
            {
                return CommandOptionType.SingleValue;
            }

            if (clrType.IsArray || typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(clrType))
            {
                return CommandOptionType.MultipleValue;
            }

            var typeInfo = clrType.GetTypeInfo();
            if (typeInfo.IsValueType)
            {
                return CommandOptionType.SingleValue;
            }

            if (typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(typeInfo))
            {
                return GetOptionType(typeInfo.GetGenericArguments().First());
            }

            throw new ArgumentException("Could not determine CommandOptionType", nameof(clrType));
        }
    }
}
