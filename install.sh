#!/bin/bash

clear

# First giving user some information about what this script actually does.

echo "================================================================================"
echo "Automatic installation script for Phosphorus Five"
echo ""
echo "Please let it finish, without interruptions, which might take some time!"
echo ""
echo "Warning, this script will install P5 'greadily' on your server,"
echo "taking over your entire Apache installation."
echo "It also expect a 'virgin' Ubuntu service, without MySQL, or anything"
echo "besides the bare minimums installed from before."
echo ""
echo "If this is not OK with you, you should end the script now!"
echo ""
echo "If you followed the default installation process, when you setup Ubuntu,"
echo "and you didn't add any extra packages, it should work flawlessly though."
echo "It has only been tested with Ubuntu Servers, and only the latest version,"
echo "which is version 'Ubuntu Server 16.04.3 LTS'."
echo "================================================================================"
echo ""

# Then asking user to confirm installation.
while true; do
    read -p "Do you still wish to install this program? " yn
    case $yn in
        [Yy]* ) break;;
        [Nn]* ) exit;;
        * ) echo "Please answer y for yes, or n for no";;
    esac
done

# Installing Apache
apt-get --assume-yes install apache2

# Informing user that his MySQL password can be found in web.config
echo "Your MySQL password can be found in the file '/var/www/html/web.config'"

# Installing MySQL server.
# Notice, by default MySQL is setup without networking, hence unless user explicitly opens it
# up later, this should be perfectly safe.
debconf-set-selections <<< 'mysql-server mysql-server/root_password password SomeRandomPassword'
debconf-set-selections <<< 'mysql-server mysql-server/root_password_again password SomeRandomPassword'
apt-get -y install mysql-server

# Installing Mono and mon_mono
apt-get --assume-yes install mono-complete
apt-get --assume-yes install libapache2-mod-mono

# Installing zip, since main P5 file is distributed as a zip file.
apt-get --assume-yes install unzip

# Download P5, and unzipping, in addition to moving it into main www/html folder.
wget https://github.com/polterguy/phosphorusfive/releases/download/v4.1/binaries.zip
unzip binaries.zip
cp -R p5/* /var/www/html

# Removing default index.html file
rm /var/www/html/index.html

# Editing web.config file, passing in the password user selected during process further up.
sed -i 's/User Id=root;/User Id=root;password=SomeRandomPassword;/g' /var/www/html/web.config

# Giving ownership (recursively) to Apache user for entire folder.
# Necessary since P5 will create and modify its own file structure.
chown -R www-data:www-data /var/www/html

# Installing GnuPG, and making sure Apache has its GnuPG folder.
apt-get --assume-yes install gnupg2
mkdir /var/www/.gnupg
chown -R www-data:www-data /var/www/.gnupg

# Configuring mod_mono
echo "
<FilesMatch \"^[^\.]+$\">
    ForceType application/x-asp-net
</FilesMatch>
<Files ~ \"\.hl\">
    Order allow,deny
    Deny from all
</Files>
<Location \"/users\">
    Order allow,deny
    Deny from all
</Location>
<Location \"/common\">
    Order allow,deny
    Deny from all
</Location>
" >> /etc/apache2/mods-enabled/mod_mono_auto.conf

# Restarting Apache
service apache2 restart
