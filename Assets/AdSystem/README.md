# Ad System

AppLovin MAX-ის drop-in wrapper: interstitial + rewarded რეკლამები სუფთა
`IAdService` locator-ის უკან, auto-retry-ით და ედიტორის ტესტ-რეკლამებით.

## დამოკიდებულება

ეს package **საჭიროებს AppLovin MAX SDK-ს** პროექტში (`Assets/MaxSdk`).
ჯერ AppLovin დააყენე, მერე ეს package.

## დაყენება Git-იდან

Unity → `Window ▸ Package Manager ▸ + ▸ Install package from git URL...`:

```
https://github.com/<user>/<repo>.git?path=Assets/AdSystem
```

ან `Packages/manifest.json`-ში:

```json
"com.pelamushi.adsystem": "https://github.com/<user>/<repo>.git?path=Assets/AdSystem"
```

## გამოყენება

1. `Assets ▸ Create ▸ AdSystem ▸ Ad Config` — შექმენი `AdConfig` და ჩაწერე
   შენი ad unit ID-ები (Android / iOS).
2. დაამატე `SdkManager` სცენაში და მიაბი `AdConfig`.
3. გამოიძახე ნებისმიერი სკრიპტიდან:

```csharp
using AdSystem;

if (AdServices.IsRegistered)
{
    AdServices.Current.OnUserRewarded += GrantReward;
    AdServices.Current.ShowRewarded();
}
```

ედიტორში რეკლამები ფეიკია (ყოველთვის "ready"), build-ში — ნამდვილი AppLovin.
