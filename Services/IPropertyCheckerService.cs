﻿namespace PixelMart.API.Services;

public interface IPropertyCheckerService
{
    bool TypeHasProperties<T>(string? fields);
}
