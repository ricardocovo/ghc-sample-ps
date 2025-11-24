# GhcSamplePs.Web

Blazor Web App - UI Layer

## Purpose

This project contains the user interface layer of the GhcSamplePs application, including:
- Blazor components (`.razor` files)
- Pages and layouts
- UI-specific services and state management
- Client-side validation and user interaction logic
- **Progressive Web App (PWA) support**

## Dependencies

- **GhcSamplePs.Core** - Business logic and services
- **MudBlazor** - Material Design component library (v8.15.0)

## Project Structure

```
GhcSamplePs.Web/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
├── wwwroot/             # Static files (CSS, JS, images, PWA assets)
│   ├── manifest.json    # PWA manifest
│   ├── icon-192.png     # PWA icon (192x192)
│   └── icon-512.png     # PWA icon (512x512)
├── App.razor            # Root component (includes PWA meta tags)
├── Program.cs           # Application entry point and DI configuration
└── appsettings.json     # Configuration
```

## Progressive Web App (PWA)

The application includes PWA support to enable installation on mobile devices:

- **Manifest Link**: `App.razor` includes a link to `/manifest.json`
- **Browser Detection**: Supported browsers (Chrome, Edge, Safari 16.4+) will detect PWA capability
- **Installation**: Users can install the app via browser menu (Add to Home Screen)

### PWA Configuration

The manifest link is configured in `Components/App.razor`:
```html
<link rel="manifest" href="/manifest.json" />
```

This enables:
- Install prompt on supported browsers
- Add to home screen functionality on mobile devices
- App-like experience when launched from home screen

## Running the Application

```powershell
cd src/GhcSamplePs.Web
dotnet run
```

Visit `https://localhost:5001` in your browser.

## Progressive Web App (PWA) Support

This application is configured as a Progressive Web App, allowing users to install it on their mobile devices for an app-like experience.

### PWA Features

- ✅ **Installable**: Can be installed on Android and iOS home screens
- ✅ **Standalone Mode**: Launches without browser UI
- ✅ **App Icons**: Custom icons for home screen
- ✅ **Theme Colors**: Branded colors for mobile browsers
- ✅ **Mobile Optimized**: Responsive design for all screen sizes

### Testing PWA Installation

See the comprehensive testing guide: [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)

**Quick testing steps:**

**Android (Chrome):**
1. Open app in Chrome on Android
2. Tap menu (⋮) → "Install app"
3. Launch from home screen

**iOS (Safari 16.4+):**
1. Open app in Safari on iOS
2. Tap Share (⬆) → "Add to Home Screen"
3. Launch from home screen

### PWA Configuration Files

- **manifest.json**: App metadata, display mode, icons
- **App.razor**: PWA meta tags for Android and iOS
- **Icons**: 192x192 and 512x512 PNG files

## Development Guidelines

### Component Development

- Keep components focused and small
- Use component parameters for input: `[Parameter]`
- Use EventCallback for output events
- Avoid business logic in `.razor` files - call services instead
- Use `@inject` directive to inject services
- Use MudBlazor components instead of HTML elements where appropriate

### Service Usage

```razor
@inject IMyService MyService

@code {
    private async Task HandleClick()
    {
        var result = await MyService.DoSomethingAsync();
        // Update UI state
    }
}
```

### MudBlazor Component Usage

```razor
@* Use MudButton instead of HTML button *@
<MudButton Variant="Variant.Filled" Color="Color.Primary" OnClick="HandleClick">
    Click Me
</MudButton>

@* Use MudCard for content sections *@
<MudCard>
    <MudCardContent>
        <MudText Typo="Typo.h5">Card Title</MudText>
        <MudText>Card content goes here</MudText>
    </MudCardContent>
</MudCard>
```

### What NOT to Include

- ❌ Business rules or logic (belongs in Core)
- ❌ Direct data access (use services from Core)
- ❌ Complex calculations (use services from Core)
- ❌ Domain models (use DTOs/ViewModels)

## Styling and CSS

### CSS Files

- **app.css** - Global styles and mobile optimizations
  - Safe area insets for notched devices
  - Touch-friendly minimum sizes
  - Mobile-specific optimizations
  - No Bootstrap dependencies

- **Component-scoped CSS** - Use `.razor.css` files for component-specific styles
  - MainLayout.razor.css
  - NavMenu.razor.css

### Mobile-First Design

The application is optimized for mobile devices:

- **Safe Area Insets**: Support for notched devices (iPhone X+)
- **Touch Targets**: Minimum 44x44 pixels for all interactive elements
- **Responsive Text**: Minimum 16px font size on mobile
- **Smooth Scrolling**: Native smooth scroll behavior
- **Touch Optimizations**: No tap highlights, no long-press menus

### Responsive Breakpoints

MudBlazor uses these breakpoints:

| Breakpoint | Width | Device Type |
|-----------|-------|-------------|
| xs | 0-599px | Small phones |
| sm | 600-959px | Large phones |
| md | 960-1279px | Tablets |
| lg | 1280-1919px | Desktops |
| xl | 1920px+ | Large desktops |

## Configuration

Configuration is managed in:
- `appsettings.json` - General settings
- `appsettings.Development.json` - Development-specific settings
- `Program.cs` - Service registration and middleware
- `manifest.json` - PWA configuration

## Adding New Features

1. Define service interface and implementation in `GhcSamplePs.Core`
2. Register service in `Program.cs`
3. Create Blazor component in appropriate folder (use MudBlazor components)
4. Inject and use service in component
5. Test business logic in `GhcSamplePs.Core.Tests`

## Mobile Optimization

The application is optimized for mobile devices:
- Responsive layouts (works on screens from 320px to desktop)
- Touch-friendly UI elements
- Optimized viewport settings
- PWA support for installation

## See Also

- [PWA Testing Guide](../../docs/PWA_Testing_Guide.md)
- [PWA Test Results](../../docs/PWA_Test_Results.md)
- [Architecture Guidelines](../../.github/instructions/blazor-architecture.instructions.md)
- [C# Guidelines](../../.github/instructions/csharp.instructions.md)

## Theme Configuration

### Custom MudBlazor Theme

The application uses a custom Material Design theme configured through MudBlazor. The theme is defined in `Theme/AppTheme.cs` and provides a consistent, professional appearance across all components.

#### Color Palette

**Light Mode:**
- **Primary**: Blue 700 (#1976d2) - Used for primary actions, buttons, app bar
- **Secondary**: Grey 800 (#424242) - Accent color for secondary elements
- **Tertiary**: Cyan 600 (#00acc1) - Additional accent color
- **Success**: Green 500 (#4caf50) - Success states and positive feedback
- **Warning**: Orange 500 (#ff9800) - Warning states and caution
- **Error**: Red 500 (#f44336) - Error states and destructive actions
- **Info**: Blue 500 (#2196f3) - Informational messages

**Dark Mode:**
- Lighter variants of all colors for better visibility on dark backgrounds
- True dark background (#121212) optimized for OLED displays
- Proper contrast ratios maintaining WCAG AA compliance

#### Typography

- **Font Family**: Roboto (Google's Material Design font)
- Loaded from Google Fonts CDN
- Responsive font sizes optimized for mobile-first design
- Proper line heights and letter spacing for readability

#### Layout Properties

- **Border Radius**: 4px - Material Design default for rounded corners
- **App Bar Height**: 64px - Standard height, mobile-friendly
- **Drawer Width**: 260px - Comfortable navigation width
- **Spacing**: Uses MudBlazor's built-in spacing system

#### Responsive Breakpoints

The theme uses MudBlazor's default responsive breakpoints:
- **xs**: 0-599px (small phones)
- **sm**: 600-959px (large phones)
- **md**: 960-1279px (tablets)
- **lg**: 1280-1919px (desktops)
- **xl**: 1920px+ (large desktops)

### Customizing the Theme

To modify the theme colors or layout:

1. Edit `Theme/AppTheme.cs`
2. Update color values in `PaletteLight` and/or `PaletteDark`
3. Modify `LayoutProperties` for spacing and sizing
4. Rebuild the application: `dotnet build`

### Theme Application

The theme is applied in `App.razor` through the `MudThemeProvider` component:

```razor
<MudThemeProvider Theme="@_theme" />
```

All MudBlazor components automatically use the configured theme colors and styles.

## Authentication and Authorization

### Azure Entra ID Integration

This application uses **Azure Entra ID External Identities** for authentication and authorization. The infrastructure must be set up before the application can authenticate users.

#### Authentication Features

- ✅ **Multiple Identity Providers**: Microsoft Account, Google, Email signup
- ✅ **Self-Service Registration**: User sign-up flows
- ✅ **Role-Based Access Control**: Admin and User roles
- ✅ **Secure Token Management**: OAuth 2.0 / OpenID Connect
- ✅ **Single Sign-On**: Seamless authentication experience

#### Configuration

Authentication configuration is stored in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your-tenant.onmicrosoft.com",
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "00000000-0000-0000-0000-000000000000",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

**Security Note**: Client Secret must be stored in:
- **Development**: User Secrets (`dotnet user-secrets set "AzureAd:ClientSecret" "your-secret"`)
- **Production**: Azure Key Vault (referenced via environment variables)

#### Setup Requirements

Before running the application with authentication:

1. **Azure Infrastructure Setup**
   - Follow: [Azure Entra ID Setup Guide](../../docs/Azure_EntraID_Setup_Guide.md)
   - Create Entra ID tenant and application registration
   - Configure identity providers and user flows
   - Set up Azure Key Vault for secrets

2. **Development Environment Configuration**
   - Follow: [Development Environment Setup](../../docs/Development_Environment_Setup.md)
   - Update `appsettings.Development.json` with Tenant ID and Client ID
   - Store Client Secret in user secrets
   - Verify redirect URIs in Azure Portal

3. **Verification**
   - Complete: [Infrastructure Verification Checklist](../../docs/Infrastructure_Verification_Checklist.md)
   - Test authentication in Azure Portal
   - Verify configuration values

#### Quick Reference

For quick access to configuration values and commands, see:
- [Configuration Quick Reference](../../docs/Azure_EntraID_Configuration_Reference.md)

#### Authentication Code Implementation

**Status**: 🔄 Pending - Authentication middleware and UI components not yet implemented

The authentication code integration will be completed in a future phase. Current configuration templates are ready for when authentication code is added.

**Next Steps:**
1. Add Microsoft.Identity.Web NuGet packages
2. Configure authentication middleware in `Program.cs`
3. Create LoginDisplay component
4. Add Authorize attributes to protected pages
5. Implement authorization policies

See specification: [Entra ID External Identities Integration](../../docs/specs/EntraID_ExternalIdentities_Integration_Specification.md)
