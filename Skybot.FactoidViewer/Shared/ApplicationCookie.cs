﻿namespace Skybot.FactoidViewer.Shared;

public sealed class ApplicationCookie
{
    private ApplicationCookie(string name) => Name = name;

    /// <summary>
    /// Gets the anti-forgery cookie.
    /// </summary>
    public static ApplicationCookie Antiforgery => new("_anti-forgery");

    /// <summary>
    /// Gets the application identity cookie.
    /// </summary>
    public static ApplicationCookie Application => new("skybot-factoid-viewer");

    /// <summary>
    /// Gets the identity correlation cookie.
    /// </summary>
    public static ApplicationCookie Correlation => new("skybot-factoid-viewer-auth-correlation");

    /// <summary>
    /// Gets the external identity cookie.
    /// </summary>
    public static ApplicationCookie External => new("skybot-factoid-viewer-auth-external");

    /// <summary>
    /// Gets the identity state cookie.
    /// </summary>
    public static ApplicationCookie State => new("skybot-factoid-viewer-auth-state");

    /// <summary>
    /// Gets the name of the cookie.
    /// </summary>
    public string Name { get; }
}
