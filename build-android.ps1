# ShowMeWhere Android Build Script
# Builds Debug/Release APK for local testing or distribution

param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    
    [switch]$Package,  # Create standalone APK (default is unpackaged for local testing)
    [switch]$Install,  # Deploy to connected Android device
    [switch]$Clean
)

$projectPath = "src/ShowMeWhere.App/ShowMeWhere.App.csproj"
$framework = "net10.0-android"

if ($Clean) {
    Write-Host "🧹 Cleaning Android build artifacts..." -ForegroundColor Cyan
    dotnet clean $projectPath -f $framework
    Remove-Item -Path "src/ShowMeWhere.App/bin/android-*" -Recurse -ErrorAction SilentlyContinue
    Remove-Item -Path "src/ShowMeWhere.App/obj/android-*" -Recurse -ErrorAction SilentlyContinue
}

Write-Host "📱 Building Android app ($Configuration)..." -ForegroundColor Cyan

if ($Package) {
    Write-Host "📦 Creating APK package..." -ForegroundColor Cyan
    $buildArgs = @(
        'publish',
        $projectPath,
        '-f', $framework,
        '-c', $Configuration,
        '-p:AndroidPackageFormat=apk'
    )
} else {
    $buildArgs = @(
        'build',
        $projectPath,
        '-f', $framework,
        '-c', $Configuration
    )
}

$buildArgs += '--no-restore'

dotnet @buildArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build succeeded!" -ForegroundColor Green
    
    if ($Install) {
        Write-Host "📲 Installing to connected device..." -ForegroundColor Cyan
        dotnet build $projectPath -f $framework -c $Configuration -t Install
    }
    
    if ($Package) {
        $outputPath = "src/ShowMeWhere.App/bin/$Configuration/$framework/publish"
        $apkPath = Get-ChildItem -Path $outputPath -Filter "*.apk" -Recurse | Select-Object -First 1
        if ($apkPath) {
            Write-Host "`n📦 APK Location: $($apkPath.FullName)" -ForegroundColor Green
            Write-Host "ℹ️  Share this APK with testers or install manually:" -ForegroundColor Cyan
            Write-Host "   adb install -r `"$($apkPath.FullName)`"" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
