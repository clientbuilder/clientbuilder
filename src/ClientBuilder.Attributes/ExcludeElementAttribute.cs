﻿using System;

namespace ClientBuilder.Attributes;

/// <summary>
/// Attribute that identifies application element that must be excluded from the Client Builder assembly scanning.
/// </summary>
[AttributeUsage(AttributeTargets.All)]
public class ExcludeElementAttribute : Attribute
{
}