#
# MAKEFILE
#

TARGETNAME=CopyWindowsSpotLight

#�^�[�Q�b�g�̃^�C�v(exe or lib or dll)
TARGETTYPE=exe

#�o�͐�̓J�����g�f�B���N�g���֎~
OUTDIR=.

#�t���O
CSFLAGS=/nologo

##�ݒ肱���܂�##################################
TARGET=$(OUTDIR)\$(TARGETNAME).$(TARGETTYPE)

CSC=csc.exe

ALL : $(TARGET)

CLEAN :
	-@erase /Q $(OUTDIR)\*.$(TARGETTYPE)

$(TARGET) : CopyWindowsSpotLight.cs
	$(CSC) $(CSFLAGS) /OUT:$@ $**
