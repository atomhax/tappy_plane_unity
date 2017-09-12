from __future__ import print_function
# How to use:
# Add a new 'Run Script' phase - ON TOP - of the Build Phases tab using the following Shell command and build the project: /usr/bin/python Spil.framework/setup.py $(PROJECT_NAME)
# Or run the shell command from the terminal using the project name: python Spil.framework/setup.py <ProjectName> <UseICloudContainer> <useICloudKV> <UsePushNotifications>

import datetime
import os
import re
import plistlib
import shutil
import sys
if sys.hexversion >= 0x3000000:
	from builtins import str
from mod_pbxproj import XcodeProject

# font consts
RED = '\033[91m'
YELLOW = '\033[93m'
END = '\033[0m'
BOLD = '\033[1m'

# helper methods
def addBundleResource(src, dst, group):
	# check if the resource actually exists
	if not os.path.exists(src):
		return

	# copy the dir or file
	if os.path.isdir(src):
		if os.path.exists(dst):
			shutil.rmtree(dst)
		shutil.copytree(src, dst)
	else:
		shutil.copy2(src, dst)
	
	# add it to the xcode project	
	project.add_file_if_doesnt_exist(dst, parent=bundles, weak=False)
	return

def str2bool(v):
  return v.lower() in ("yes", "true", "t", "1")

def inplace_change(filename, old_string, new_string):
    # Safely read the input filename using 'with'
    with open(filename) as f:
        s = f.read()
        if old_string not in s:
            print('"{old_string}" not found in {filename}.'.format(**locals()))
            return

    # Safely write the changed content, if found in the file
    with open(filename, 'w') as f:
        print('Changing "{old_string}" to "{new_string}" in {filename}'.format(**locals()))
        s = s.replace(old_string, new_string)
        f.write(s)
        
# determine if the spil sdk is already initialized
if os.path.exists(os.getcwd() + '/spil.initialized'):
	print('Spil SDK was already initialized!')
	exit(0)

# check number of arguments
if len(sys.argv) < 2:
	print(RED + BOLD + 'ERROR: Wrong arguments! Usage: python spilsdksetup.py <ProjectName> <ExportSpilGamesEntitlements> <UseICloudContainer> <useICloudKV> <UsePushNotifications>' + END)
	exit(1)
    
# try to find the project to modify
projectname = sys.argv[1]
if not os.path.isdir(projectname):
	print(RED + BOLD + projectname + '.xcodeproj not found!' + END)
	exit(1)
else:
	print('Modifying XCode project: ' + projectname)

# read entitlements arguments
exportSpilGamesEntitlements = str2bool(sys.argv[2]) if len(sys.argv) > 2 else False;
useICloudContainer = str2bool(sys.argv[3]) if len(sys.argv) > 3 else False; # deprecated
useICloudKV = str2bool(sys.argv[4]) if len(sys.argv) > 4 else False;
usePushNotifications = str2bool(sys.argv[5]) if len(sys.argv) > 5 else False; # deprecated
print("exportSpilGamesEntitlements: " + str(exportSpilGamesEntitlements));
print("useICloudContainer: " + str(useICloudContainer));
print("useICloudKV: " + str(useICloudKV));
print("usePushNotifications: " + str(usePushNotifications));

# paths
projectpath = projectname + '.xcodeproj/'
projectfilename = 'project.pbxproj'
backupPath = os.getcwd() + '/ProjectBackups/'
plistPath = os.getcwd() + '/' + projectname + '/info.plist'
altPlistPath = os.getcwd() + '/info.plist'
entitlementsPath = os.getcwd() + '/' + projectname + '/' + projectname + '.entitlements'
altEntitlementsPath = os.getcwd() + '/' + projectname + '.entitlements'

# --- load the project file ---
project = XcodeProject.Load(projectpath + projectfilename)

# backup the project first
print('Creating project backup')
if not os.path.exists(backupPath):
    os.makedirs(backupPath)
sourcePath = os.path.abspath(projectpath + projectfilename)
destPath = backupPath + "%s.%s.backup" % (projectfilename, datetime.datetime.now().strftime('%d%m%y-%H%M%S'))
shutil.copy2(sourcePath, destPath)

print('Cleaning unity export')
# remove spil.framework from the data raw directory if it was exported by unity there
project.remove_file_by_path('Frameworks/Plugins/iOS/Spil.framework')
# remove any meta files if it was exported by unity
for root, dirs, files in os.walk(os.getcwd() + "/Spil.framework"):
	for file in files:
		if file.endswith(".meta"):
			os.remove(os.path.join(root, file))

# add system frameworks
print('Adding system frameworks')
frameworks = project.get_or_create_group('Frameworks')
requiredSystemFrameworks = ['Accounts', 'AdSupport', 'AssetsLibrary', 'AudioToolbox', 'AVFoundation', 'EventKit', 'EventKitUI', 'CoreData', 'CoreGraphics', 
							'CoreLocation', 'CoreMedia', 'CoreMotion', 'CoreTelephony', 'iAd', 'MapKit', 'MediaPlayer', 'MessageUI', 'QuartzCore', 
							'Social', 'StoreKit', 'SystemConfiguration', 'Twitter', 'WebKit']
for framework in requiredSystemFrameworks:
	project.add_file_if_doesnt_exist('System/Library/Frameworks/' + framework + '.framework', parent=frameworks, weak=False, tree='SDKROOT')
requiredSystemLibraries = ['libxml2', 'libz','libsqlite3', 'libc++']
for library in requiredSystemLibraries:
	project.add_file_if_doesnt_exist('usr/lib/' + library + '.tbd', parent=frameworks, weak=False, tree='SDKROOT')

# add custom frameworks
print('Adding custom frameworks')
requiredCustomFrameworks = ['AdjustSdk', 'Chartboost', 'FBAudienceNetwork', 'FacebookAdapter', 'Fyber_AdColony', 'Fyber_UnityAds', 'Fyber_Vungle', 'GoogleMobileAds', 'MMAdSDK', 'UnityAds', 'ZendeskSDK', 'ZendeskProviderSDK', 'FirebaseCore', 'FirebaseInstanceID', 'FirebaseAnalytics', 'FirebaseDynamicLinks', 'GoogleToolboxForMac']
project.add_file_if_doesnt_exist('Spil.framework', parent=frameworks, weak=False)
for framework in requiredCustomFrameworks:
	project.add_file_if_doesnt_exist('Spil.framework/Frameworks/' + framework + '.framework', parent=frameworks, weak=False)
#requiredCustomLibraries = ['libFyberSDK' ] # libAdapterSDKMoPub, libAdMobMMAdapter
#for library in requiredCustomLibraries:
#	project.add_file_if_doesnt_exist('Spil.framework/Frameworks/' + library + '.a', parent=frameworks, weak=True)

# copying resources
print('Copying resources and adding them to the XCode project')
bundles = project.get_or_create_group('')
addBundleResource(os.getcwd() + '/Spil.framework/Settings.bundle', os.getcwd() + '/Settings.bundle', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/Frameworks/Fyber_UnityAds.framework/Resources/UnityAds.bundle', os.getcwd() + '/UnityAds.bundle', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/MRAID.bundle', os.getcwd() + '/MRAID.bundle', bundles)
addBundleResource(os.getcwd() + '/Data/Raw/defaultGameConfig.json', os.getcwd() + '/defaultGameConfig.json', bundles)
addBundleResource(os.getcwd() + '/Data/Raw/defaultGameData.json', os.getcwd() + '/defaultGameData.json', bundles)
addBundleResource(os.getcwd() + '/Data/Raw/defaultPlayerData.json', os.getcwd() + '/defaultPlayerData.json', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/ZendeskSDK.bundle', os.getcwd() + '/ZendeskSDK.bundle', bundles)
addBundleResource(os.getcwd() + '/Spil.framework/ZendeskSDKStrings.bundle', os.getcwd() + '/ZendeskSDKStrings.bundle', bundles)

# add png/xib/der resources
for file in os.listdir(os.getcwd() + '/Spil.framework'):
    if file.lower().endswith(".png") or file.lower().endswith(".xib") or file.lower().endswith(".der"):
        addBundleResource(os.getcwd() + '/Spil.framework/' + file, os.getcwd() + '/' + file, bundles)

# change build settings
print('Modifying project build settings')
project.add_single_valued_flag('ENABLE_BITCODE', 'NO')
project.add_other_ldflags(['-ObjC', '-Wl,-U,_UnitySendMessage'])
project.add_framework_search_paths('$(PROJECT_DIR)', recursive=False)
project.add_framework_search_paths('$(PROJECT_DIR)/Spil.framework/Frameworks', recursive=False)

# --- try to find the info plist ---
currentPlistPath = plistPath;
if not os.path.isfile(plistPath):
	currentPlistPath = altPlistPath;
	if not os.path.isfile(altPlistPath):
		print(RED + BOLD + plistPath + ' not found!' + END);
		exit(1);
print("info.plist found at: " + currentPlistPath);

# backup info.plist first
print('Creating info.plist backup')
sourcePath = os.path.abspath(currentPlistPath)
destPath = backupPath + "info.plist.%s.backup" % (datetime.datetime.now().strftime('%d%m%y-%H%M%S'))
shutil.copy2(sourcePath, destPath)

# modify plist
print('Modifying info.plist')
plist = plistlib.readPlist(currentPlistPath)
plist['NSAppTransportSecurity'] = dict(NSAllowsArbitraryLoads = True)
plist['UIBackgroundModes'] = ["remote-notification"]
plist['NSCameraUsageDescription'] = "Used to take a photo."
plist['NSCalendarsUsageDescription'] = "Used to access the calendar."
plist['NSPhotoLibraryUsageDescription'] = "Used to access the photo library."
plist['NSAppleMusicUsageDescription'] = "Used to access apple music."
plist['NSBluetoothPeripheralUsageDescription'] = "Used to communicate between devices using Bluetooth."
plist['NSContactsUsageDescription'] = "Used to access the phone contacts."
plist['NSHealthShareUsageDescription'] = "Used to share data with the health app."
plist['NSHomeKitUsageDescription'] = "Used to access the domotica app information."
plist['NSLocationAlwaysUsageDescription'] = "Used to access the user location."
plist['NSLocationWhenInUseUsageDescription'] = "Used to access the user location."
plist['NSMicrophoneUsageDescription'] = "Used to access the microphone."
plist['NSMotionUsageDescription'] = "Used to access the phones motion information."
plist['NSRemindersUsageDescription'] = "Used to access the reminders app information."
plist['NSSiriUsageDescription'] = "Siri is used for voice control."
plist['NSSpeechRecognitionUsageDescription'] = "Used to enable speech recognition."

# write plist
print('Saving info.plist')
plistlib.writePlist(plist, currentPlistPath)

#  --- try to find the entitlements plist ---
if exportSpilGamesEntitlements:
	entitlementsFileCreated = False;
	currentEntitlementsPath = entitlementsPath;
	if not os.path.isfile(entitlementsPath):
		currentEntitlementsPath = altEntitlementsPath;
		if not os.path.isfile(altEntitlementsPath):
			newFile = open(currentEntitlementsPath, 'a');
			newFile.write('<plist version="1.0"><dict></dict></plist>');
			newFile.close();
			entitlementsFileCreated = True;
	if entitlementsFileCreated:
		print(projectname + ".entitlements created at: " + currentEntitlementsPath);
	else:
		print(projectname + ".entitlements found at: " + currentEntitlementsPath);

	# backup <projectname>.entitlements first
	print('Creating ' + projectname + '.entitlements backup')
	sourceEntitlementsPath = os.path.abspath(currentEntitlementsPath)
	destEntitlementsPath = backupPath + projectname + ".entitlements.%s.backup" % (datetime.datetime.now().strftime('%d%m%y-%H%M%S'))
	shutil.copy2(sourceEntitlementsPath, destEntitlementsPath)

	# modify entitlements plist
	print('Modifying ' + projectname + '.entitlements')
	plist = plistlib.readPlist(currentEntitlementsPath)

	# add shared application group
	plist['com.apple.security.application-groups'] = ["group.com.spilgames"];

	# add iCloud kv store
	if useICloudKV:
		plist['com.apple.developer.ubiquity-kvstore-identifier'] = "$(TeamIdentifierPrefix)$(CFBundleIdentifier)";

	# write entitlements plist
	print('Saving ' + projectname + '.entitlements')
	plistlib.writePlist(plist, currentEntitlementsPath)

	# add the entitlements plist file to the xcode project
	project.add_file_if_doesnt_exist(currentEntitlementsPath, parent=None, weak=False)
	project.add_single_valued_flag('CODE_SIGN_ENTITLEMENTS', projectname + '.entitlements')

# save the XCode project file
print('Saving the XCode project file')
project.save()

#inplace_change(os.getcwd() + '/Unity-iPhone.xcodeproj/project.pbxproj', "enabled = 0", "enabled = 1");

# --- mark setup as done ---
open(os.getcwd() + '/spil.initialized', 'a').close()
print('Done!')