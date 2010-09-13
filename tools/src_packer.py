import sys
import getopt
import shutil
import fnmatch
import os
import fnmatch
import zipfile
import glob

# полностью процедурный код

# --from path
# --to path

fromPath = "a"
toPath = "b"
zipPath = "src.zip"
IsDirectZip = False     # incomplete: есть сложности с созданием директорий...
excludeDirs = [".svn", "debug", "release"]
excludeFiles = [".obj", "*.suo", "*.user", "*.ncb", "*.pdb", "*.idb", "*.map"]

def main(argv):
    getPaths(argv)
    copyFilesTree(fromPath, toPath, getExcludedFiles, IsDirectZip)
    if not IsDirectZip:
        createZip(toPath)

def createZip(toPath):
    zip = zipfile.ZipFile(zipPath, 'w', zipfile.ZIP_DEFLATED)
    appendZipFile(zip, toPath)
    zip.close()

def appendZipFile(zip, name):
    for fname in glob.glob(name + "/*"):
#        print fname
        if os.path.isdir(fname):
            appendZipFile(zip, fname)
        else:
            zip_name = fname[len(toPath):]
            if zip_name:
                zip.write(fname, zip_name, zipfile.ZIP_DEFLATED)


def getPaths(argv):
    try:
        opts, args = getopt.getopt(argv, "f:t:z:d", ["from=", "to=", "zip="])  # выгребаем нужные нам параметры командной строки
    except getopt.GetoptError:
        usage()
        sys.exit(2)

    global fromPath
    global toPath
    global zipPath
    global IsDirectZip
    for opt, arg in opts:
        if opt in ("-f", "--from"):
            fromPath = arg
        elif opt in ("-t", "--to"):
            toPath = arg
        elif opt in ("-z", "--zip"):
            zipPath = arg
        elif opt == "-d":
            IsDirectZip = True

def copyFilesTree(src,dest,exclude,directZip):
    print "Start copying"
    copytree(src,dest,exclude,directZip)
    print "--- DONE ---"

def testNoCopy(src,names):
    print names
    return []

# возвращает список тех файлов и директорий, которые копировать не нужно
def getExcludedFiles(src,names):
    bad_files = []
    for name in names:
        lowName = name.lower();
        fullname = "/".join([src,name])

        if os.path.isfile(fullname):
            #print "file: " + fullname
            for pattern in excludeFiles:
                if fnmatch.fnmatch(name, pattern):
                    bad_files.append(name)
                    break
        elif os.path.isdir(fullname):
            #print "directory: " + fullname
            for pattern in excludeDirs:
                if fnmatch.fnmatch(name, pattern):      # если совпадает с каким-то wildcard, то не копируем данный файл
                    bad_files.append(name)
                    break
    return bad_files

##def zipMakedirs(zip, path):
##    normPath = path.replace("\\","/")
##    dirs = normPath.split("/")
##    curPath = ""
##    for dir in dirs:
##        curPath = curPath + dir + "/"
##        zip.write(curPath, curPath, zipfile.ZIP_DEFLATED)

class Error(EnvironmentError):
    pass

# скопировал функцию, чтобы работало в Python 2.5:
# изменил так, чтобы запись шла сразу в ZIP
def copytree(src, dst, ignore=None, directZip=False, ex_zip=None):
    names = os.listdir(src)
    if ignore is not None:
        ignored_names = ignore(src, names)
    else:
        ignored_names = set()

    if directZip:
        if ex_zip == None:
            zip = zipfile.ZipFile(zipPath, 'w', zipfile.ZIP_DEFLATED)
        else:
            zip = ex_zip

    if not directZip:
        os.makedirs(dst)

    errors = []
    for name in names:
        if name in ignored_names:
            continue
        srcname = os.path.join(src, name)
        dstname = os.path.join(dst, name)
        try:
            #if symlinks and os.path.islink(srcname):
            #    linkto = os.readlink(srcname)
            #    os.symlink(linkto, dstname)
            #elif
            if os.path.isdir(srcname):
                copytree(srcname, srcname, ignore, directZip, zip)
            else:
                if directZip:
                    zname = srcname[len(fromPath):]
                    zip.write(srcname, zname, zipfile.ZIP_DEFLATED)
                else:
                    shutil.copy2(srcname, dstname)
            # XXX What about devices, sockets etc.?
        except (IOError, os.error), why:
            errors.append((srcname, dstname, str(why)))
        # catch the Error from the recursive copytree so that we can
        # continue with other files
        except Error, err:
            errors.extend(err.args[0])
    try:
        shutil.copystat(src, dst)
    except WindowsError:
        # can't copy file access times on Windows
        pass
    except OSError, why:
        errors.extend((src, dst, str(why)))
    if errors:
        raise Error(errors)

    if directZip and ex_zip == None:
        zip.close()


if __name__ == "__main__":
    main(sys.argv[1:])