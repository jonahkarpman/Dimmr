# Generates dimmr.ico: a phosphor-green brightness/contrast glyph on a dark rounded tile.
# Run from the repo root: pwsh -File tools/make-icon.ps1
Add-Type -AssemblyName System.Drawing

$size = 256
$bmp = New-Object System.Drawing.Bitmap $size, $size
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

$near = [System.Drawing.Color]::FromArgb(255, 10, 10, 10)
$accent = [System.Drawing.Color]::FromArgb(255, 0, 255, 65)
$phos = [System.Drawing.Color]::FromArgb(255, 51, 255, 51)

# Brightness/contrast glyph only, on a transparent background:
# a circle outline with the right half filled.
$cx = 128; $cy = 128; $r = 98
$circle = New-Object System.Drawing.Rectangle ($cx - $r), ($cy - $r), ($r * 2), ($r * 2)
$penC = New-Object System.Drawing.Pen $phos, 16
$g.DrawEllipse($penC, $circle)
$brushC = New-Object System.Drawing.SolidBrush $phos
$g.FillPie($brushC, $circle, -90, 180)

$g.Dispose()

# Wrap the PNG in a single-image .ico
$ms = New-Object System.IO.MemoryStream
$bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
$png = $ms.ToArray()

$out = Join-Path $PSScriptRoot '..\src\Dimmr\dimmr.ico'
$fs = [System.IO.File]::Open((Resolve-Path (Split-Path $out)).Path + '\dimmr.ico', [System.IO.FileMode]::Create)
$bw = New-Object System.IO.BinaryWriter $fs
$bw.Write([UInt16]0)              # reserved
$bw.Write([UInt16]1)              # type: icon
$bw.Write([UInt16]1)              # image count
$bw.Write([Byte]0)               # width (0 = 256)
$bw.Write([Byte]0)               # height (0 = 256)
$bw.Write([Byte]0)               # palette
$bw.Write([Byte]0)               # reserved
$bw.Write([UInt16]1)              # color planes
$bw.Write([UInt16]32)            # bits per pixel
$bw.Write([UInt32]$png.Length)   # image size
$bw.Write([UInt32]22)            # offset to image
$bw.Write($png)
$bw.Flush()
$fs.Close()

Write-Output ("wrote " + ((Resolve-Path (Split-Path $out)).Path + '\dimmr.ico'))
