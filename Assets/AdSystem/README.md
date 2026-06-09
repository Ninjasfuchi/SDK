# Ad System

AppLovin MAX-ის drop-in wrapper: interstitial + rewarded რეკლამები სუფთა
`IAdService` locator-ის უკან, auto-retry-ით და ედიტორის ტესტ-რეკლამებით.

## დამოკიდებულება

ეს package საჭიროებს ორ SDK-ს პროექტში (ჯერ ეს, მერე AdSystem):

1. **AppLovin MAX SDK** (`Assets/MaxSdk`) — რეკლამები
2. **GameAnalytics Unity SDK** — ანალიტიკა
   `https://download.gameanalytics.com/unity/GA_SDK_UNITY.unitypackage`
   დაყენების შემდეგ: `Window ▸ GameAnalytics ▸ Select Settings` და ჩაწერე
   შენი **Game Key / Secret Key** (Android + iOS).

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

## ანალიტიკა

`SdkManager` ავტომატურად ააქტიურებს GameAnalytics-ს build-ში — **D1/D7/D30
retention, DAU/MAU და სესიები თავისით ითვლება** dashboard-ში, კოდი არ სჭირდება.
ad show / reward event-ებიც ავტომატურად იწერება.

შენი custom event-ებისთვის:

```csharp
using AdSystem;

Analytics.LogEvent("level_complete", 7);
```

ედიტორში ანალიტიკა no-op-ია (მონაცემებს არ ბინძურებს).
