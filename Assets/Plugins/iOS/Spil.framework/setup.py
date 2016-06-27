# How to use:
# Add a new 'Run Script' phase - ON TOP - of the Build Phases tab using the following Shell command and build the project: /usr/bin/python Spil.framework/setup.py $(PROJECT_NAME)
# Or run the shell command from the terminal using the project name: python Spil.framework/setup.py <ProjectName>

import datetime
import os
import re
import plistlib
import shutil
import sys
from mod_pbxproj import XcodeProject

# font consts
RED = '\033[91m'
YELLOW = '\033[93m'
END = '\033[0m'
BOLD = '\033[1m'

# helper methods
def addBundleResource(src, dst, group):
	if os.path.isdir(src):
		if os.path.exists(dst):
			shutil.rmtree(dst)
		shutil.copytree(src, dst)
	else:
		shutil.copy2(src, dst)	
	project.add_file_if_doesnt_exist(dst, parent=bundles, weak=False)
	return

# determine if the spil sdk is already initialized
if os.path.exists(os.getcwd() + '/spil.initialized'):
	print 'Spil SDK was already initialized!'
	exit(0)
	
# try to find the project to modify
if len(sys.argv) < 2:
    print RED + BOLD + 'ERROR: Missing project name! usage: python spilsdksetup.py <ProjectName>' + END
    exit(1)
else:
    projectname = sys.argv[1]
    if not os.path.isdir(projectname):
		print RED + BOLD + projectname + '.xcodeproj not found!' + END
		exit(1)
    else:
		print 'Modifying XCode project: ' + projectname

# paths
projectpath = projectname + '.xcodeproj/'
projectfilename = 'project.pbxproj'
backupPath = os.getcwd() + '/ProjectBackups/'
plistPath = os.getcwd() + '/' + projectname + '/info.plist'
altPlistPath = os.getcwd() + '/info.plist'
entitlementsPath = os.getcwd() + '/' + projectname + '/' + projectname + '.entitlements'
altEntitlementsPath = os.getcwd() + '/' + projectname + '.plist'

# load the project file
project = XcodeProject.Load(projectpath + projectfilename)

# backup the project first
print 'Creating project backup'
if not os.path.exists(backupPath):
    os.makedirs(backupPath)
sourcePath = os.path.abspath(projectpath + projectfilename)
destPath = backupPath + "%s.%s.backup" % (projectfilename, datetime.datetime.now().strftime('%d%m%y-%H%M%S'))
shutil.copy2(sourcePath, destPath)

print 'Cleaning unity export'
# remove spil.framework from the data raw directory if it was exported by unity there
project.remove_file_by_path('Frameworks/Plugins/iOS/Spil.framework')
# remove any meta files if it was exported by unity
for root, dirs, files in os.walk(os.getcwd() + "/Spil.framework"):
	for file in files:
		if file.endswith(".meta"):
			os.remove(os.path.join(root, file))

# add system frameworks
print 'Adding system frameworks'
frameworks = project.get_or_create_group('Frameworks')
requiredSystemFrameworks = ['Accounts', 'AdSupport', 'AssetsLibrary', 'AudioToolbox', 'AVFoundation', 'EventKit', 'EventKitUI', 'CoreData', 'CoreGraphics', 
							'CoreLocation', 'CoreMedia', 'CoreMotion', 'CoreTelephony', 'MapKit', 'MediaPlayer', 'MessageUI', 'QuartzCore', 
							'Social', 'StoreKit', 'SystemConfiguration', 'Twitter', 'WebKit']
for framework in requiredSystemFrameworks:
	project.add_file_if_doesnt_exist('System/Library/Frameworks/' + framework + '.framework', parent=frameworks, weak=False, tree='SDKROOT')
requiredSystemLibraries = ['libxml2', 'libz','libsqlite3', 'libc++']
for library in requiredSystemLibraries:
	project.add_file_if_doesnt_exist('usr/lib/' + library + '.tbd', parent=frameworks, weak=False, tree='SDKROOT')

# add custom frameworks
print 'Adding custom frameworks'
requiredCustomFrameworks = ['Chartboost', 'FBAudienceNetwork', 'Fyber_AdColony', 'Fyber_AppLovin', 'Fyber_UnityAds', 'GoogleMobileAds', 'MMAdSDK']
project.add_file_if_doesnt_exist('Spil.framework', parent=frameworks, weak=False)
for framework in requiredCustomFrameworks:
	project.add_file_if_doesnt_exist('Spil.framework/Frameworks/' + framework + '.framework', parent=frameworks, weak=False)
#requiredCustomLibraries = ['libFyberSDK' ] # libAdapterSDKMoPub, libAdMobMMAdapter
#for library in requiredCustomLibraries:
#	project.add_file_if_doesnt_exist('Spil.framework/Frameworks/' + library + '.a', parent=frameworks, weak=True)

# copying resources
print 'Copying resources and adding them to the XCode project'
bundles = project.get_or_create_group('')
addBundleResource(os.getcwd() + '/Spil.framework/Settings.bundle', os.getcwd() + '/Settings.bundle', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/UnityAds.bundle', os.getcwd() + '/UnityAds.bundle', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/project.entitlements', os.getcwd() + '/' + projectname + '.entitlements', bundles)

# change build settings
print 'Modifying project build settings'
project.add_single_valued_flag('ENABLE_BITCODE', 'NO')
project.add_other_ldflags(['-ObjC', '-Wl,-U,_UnitySendMessage'])
project.add_framework_search_paths('$(PROJECT_DIR)', recursive=False)
project.add_framework_search_paths('$(PROJECT_DIR)/Spil.framework/Frameworks', recursive=False)

# save the project file
print 'Saving project file'
project.save()

# try to find the info plist
currentPlistPath = plistPath;
if not os.path.isfile(plistPath):
	currentPlistPath = altPlistPath;
	if not os.path.isfile(altPlistPath):
		print RED + BOLD + plistPath + ' not found!' + END;
		exit(1);
print "info.plist found at: " + currentPlistPath;

# backup info.plist first
print 'Creating info.plist backup'
sourcePath = os.path.abspath(currentPlistPath)
destPath = backupPath + "info.plist.%s.backup" % (datetime.datetime.now().strftime('%d%m%y-%H%M%S'))
shutil.copy2(sourcePath, destPath)

# modify plist
print 'Modifying info.plist'
plist = plistlib.readPlist(currentPlistPath)
plist['NSAppTransportSecurity'] = dict(NSAllowsArbitraryLoads = True)
plist['UIBackgroundModes'] = ["remote-notification"]

# write plist
print 'Saving info.plist'
plistlib.writePlist(plist, currentPlistPath)

# mark setup as done
open(os.getcwd() + '/spil.initialized', 'a').close()
print 'Done!'