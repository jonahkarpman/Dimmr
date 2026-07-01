# Generates dimmr.ico: a checked phosphor checkbox (a square with a filled inner square)
# with faint CRT scanlines, matching the in-app checkboxes.
# Run from the repo root: pwsh -File tools/make-icon.ps1
Add-Type -AssemblyName System.Drawing

$size = 256
$bmp = New-Object System.Drawing.Bitmap $size, $size
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.Clear([System.Drawing.Color]::Transparent)

$surface = [System.Drawing.Color]::FromArgb(255, 13, 26, 13)   # #0D1A0D box fill
$accent  = [System.Drawing.Color]::FromArgb(255, 0, 255, 65)   # #00FF41 border + mark

# Outer checkbox square (checked state): dark fill with a bright green border.
$margin = 24
$boxRect = New-Object System.Drawing.Rectangle $margin, $margin, ($size - 2 * $margin), ($size - 2 * $margin)
$fillBox = New-Object System.Drawing.SolidBrush $surface
$g.FillRectangle($fillBox, $boxRect)

# Inner filled square (the check mark), centered like the in-app checkbox mark.
$markSize = 112
$markPos = [int](($size - $markSize) / 2)
$fillMark = New-Object System.Drawing.SolidBrush $accent
$g.FillRectangle($fillMark, $markPos, $markPos, $markSize, $markSize)

# Faint CRT scanlines across the box interior only.
$g.SetClip($boxRect)
$scan = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(70, 0, 0, 0))
for ($y = $margin; $y -lt ($size - $margin); $y += 10) {
    $g.FillRectangle($scan, $margin, $y, ($size - 2 * $margin), 3)
}
$g.ResetClip()

# Border on top so it stays crisp over the scanlines.
$penB = New-Object System.Drawing.Pen $accent, 16
$penB.Alignment = [System.Drawing.Drawing2D.PenAlignment]::Inset
$g.DrawRectangle($penB, $boxRect)

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
