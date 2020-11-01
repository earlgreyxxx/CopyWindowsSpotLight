#
# MAKEFILE
#

TARGETNAME=CopyWindowsSpotLight

#ターゲットのタイプ(exe or lib or dll)
TARGETTYPE=exe

#出力先はカレントディレクトリ禁止
OUTDIR=.

#フラグ
CSFLAGS=/nologo

##設定ここまで##################################
TARGET=$(OUTDIR)\$(TARGETNAME).$(TARGETTYPE)

CSC=csc.exe

ALL : $(TARGET)

CLEAN :
	-@erase /Q $(OUTDIR)\*.$(TARGETTYPE)

$(TARGET) : CopyWindowsSpotLight.cs
	$(CSC) $(CSFLAGS) /OUT:$@ $**
