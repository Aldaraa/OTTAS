import React from 'react'
import { useMatches } from 'react-router-dom';
import { IoChevronForwardOutline } from 'react-icons/io5'
import css from './index.module.css'

function Breadcrumb() {
  let matches = useMatches();
  let crumbs = matches
    // first get rid of any matches that don't have handle and crumb
    .filter((match) => Boolean(match.handle?.crumb))
    // now map them into an array of elements, passing the loader
    // data to each one
    .map((match) => match.handle.crumb(match.data));

  return (
    <>
      {
        crumbs.length > 0 &&
        <div className={css.breadcrumb} >
          {crumbs.map((crumb, index) => (
            <div key={index} className={`${crumbs.length-1 === index ? 'text-white' : 'text-gray-400'} flex items-center gap-2 font-bold leading-normal`}>
              {crumb} {crumbs.length - 1 !== index && <IoChevronForwardOutline size={17}/> }
            </div> 
          ))}
        </div>
      }
    </>
  )
}

export default Breadcrumb