/*
 * phosphorus five, copyright 2014 - Mother Earth, Jannah, Gaia
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

using System;

namespace phosphorus.ajax.core
{
    /// <summary>
    /// interface for all your pages that uses the phosphorus.ajax library. instead of implementing this interface 
    /// yourself, you can inherit from the <see cref="phosphorus.ajax.AjaxPage"/>, which takes care of everything 
    /// automatically for you
    /// </summary>
    public interface IAjaxPage
    {
        /// <summary>
        /// returns the manager for your page
        /// </summary>
        /// <value>the manager</value>
        Manager Manager {
            get;
        }
    }
}

