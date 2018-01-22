#!/bin/bash

clear

# First giving user some information about what this script actually does.

echo "================================================================================"
echo "Automatic installation script for Phosphorus Five."
echo "Please let it finish, without interruptions, which might take some time."
echo "Notice, Phosphorus Five is licensed as GPLv3."
echo ""
echo "This script will also update, upgrade, and further secure your system."
echo ""
echo "The software is distributed in the hope that it will be useful,"
echo "but WITHOUT ANY WARRANTY; without even the implied warranty of"
echo "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the"
echo "GNU General Public License for more details."
echo "================================================================================"
echo ""

# Asking user if he wants to continue.
read -p "Do you wish to proceed? [y/n] " yn
if [[ ! $yn =~ ^[Yy]$ ]]; then
  exit
fi

# Making sure we update and upgrade our server.
sudo apt-get update
sudo apt-get upgrade
sudo apt-get dist-upgrade

# Downloading the latest release.
wget https://github.com/polterguy/phosphorusfive/releases/download/v7.0/binaries.zip

# Installing MySQL server.
# Notice, by default MySQL is setup without networking, hence unless user explicitly opens it.
# up later, this should be perfectly safe.
sudo debconf-set-selections <<< 'mysql-server mysql-server/root_password password SomeRandomPassword'
sudo debconf-set-selections <<< 'mysql-server mysql-server/root_password_again password SomeRandomPassword'
sudo apt-get --assume-yes install apache2 mysql-server libapache2-mod-mono unzip gnupg2 ufw

# Disabling mod_mono_auto to make sure we create an "advanced" configuration for mod_mono.
sudo a2dismod mod_mono_auto

# Turning on firewall for everything except SSH, HTTP and HTTPS
sudo ufw allow 80
sudo ufw allow 443
sudo ufw allow 22
sudo ufw enable

# Removing any old files.
# Notice, we don't remove "/common" and "/users" here.
# This allows for a nice upgrading process (hopefully) without loosing old data in your system.
sudo rm /var/www/html/index.html
sudo rm /var/www/html/Default.aspx
sudo rm /var/www/html/Global.asax
sudo rm /var/www/html/README.md
sudo rm /var/www/html/startup.hl
sudo rm -r -f /var/www/html/bin
sudo rm -r -f /var/www/html/modules/desktop
sudo rm -r -f /var/www/html/modules/bazar
sudo rm -r -f /var/www/html/modules/micro

# Creating a temporary folder to hold output.
mkdir p5

# Unzipping P5, in addition to moving it into main www/html folder, and making sure
# the Apache user has full control over folders.
unzip binaries.zip -d p5
sudo cp -R p5/* /var/www/html

# Removing both zip file, and temp folder created during above process.
rm -f binaries.zip
rm -f -r p5

# Editing web.config file, making sure we get the password correctly.
sudo sed -i 's/User Id=root;/User Id=root;password=SomeRandomPassword;/g' /var/www/html/web.config

# Making GnuPG folder for Apache process.
sudo mkdir /var/www/.gnupg    

# Giving ownership (recursively) to Apache user for entire folder.
# Necessary since P5 will create and modify its own file structure.
sudo chown -R www-data:www-data /var/www

# Configuring mod_mono app
sudo echo "
<apps>
        <web-application>
                <name>P5</name>
                <vpath>/</vpath>
                <path>/var/www/html</path>
                <vhost>example.com</vhost>
        </web-application>
</apps>
" > /etc/mono-server4/p5.webapp

# Configuring apache to accept requests for ASP.NET, and route them into Mono
sudo echo "





#############################################################
#
# Phosphorus Five configurations
# These parts was added as a part of the install.sh script 
# while installing Phosphorus Five.
#
#############################################################

MonoAutoApplication disabled
AddHandler mono .aspx .axd .config .asax .asmx
MonoApplications \"/:/var/www/html\"
DirectoryIndex Default.aspx

# Turning OFF all options, and making sure they're impossible to override using .htaccess files, to be absolutely certain.
# These are simple security measures that further tights down our web app.
<Location />
    Options None
    AllowOverride None
</Location>

# Making sure Mono handles all requests that doesn't contain a dot (.), to make our URL rewriting logic work.
<Location ~ \"^[^\.]*$\">
    SetHandler mono
</Location>

# Making sure we never serve Hyperlambda files.
<Files ~ \"\.hl$\">
    Order allow,deny
    Deny from all
</Files>

# Notice, these next steps will make sure our users and common folders have
# the right type of protection for direct file access.
# Basically, anything in 'public' whatever is available through a direct link,
# everything else will never be served

# Making user's private documents just that.
<Location ~ \"/users/[^/]+/documents/private/\">
    Order allow,deny
    Deny from all
</Location>

# Making all temp files for users inaccessible.
<Location ~ \"/users/[^/]+/temp/\">
    Order allow,deny
    Deny from all
</Location>

# Making all common private files just that.
<Location ~ \"/common/documents/private/\">
    Order allow,deny
    Deny from all
</Location>

# Making all common temp files private
<Location ~ \"/common/temp/\">
    Order allow,deny
    Deny from all
</Location>


# Phosphorus Five, further tightening server, to make it more secure.

# Turning OFF tracing
TraceEnable off

# Turning OFF server signature and token
ServerSignature Off
ServerTokens Prod

# Turning OFF ETags, to avoid information leaking.
FileETag None
" >> /etc/apache2/apache2.conf

# Installing SSL keys, if user wants to.

# Then asking user to confirm installation.
echo "Do you wish to install an SSL keypair on your server?"
echo "This step requires a pre-configured domain and DNS record."
read -p "[y/n] " yn
if [[ $yn =~ ^[Yy]$ ]]; then
  sudo apt-get install software-properties-common
  sudo add-apt-repository ppa:certbot/certbot
  sudo apt-get update
  sudo apt-get install python-certbot-apache
  sudo certbot --apache
fi

# Restarting Apache.
sudo service apache2 restart

# Informing user that his MySQL password can be found in web.config.
echo "Your MySQL password can be found in the file '/var/www/html/web.config'"
