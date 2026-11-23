// Lihtsad jooneikoonid (eraldi emojid asemel)
const base = {
  viewBox: '0 0 24 24',
  fill: 'none',
  stroke: 'currentColor',
  strokeWidth: 2,
  strokeLinecap: 'round',
  strokeLinejoin: 'round',
  'aria-hidden': true,
}

export function SearchIcon({ className }) {
  return (
    <svg {...base} className={className}>
      <circle cx="11" cy="11" r="7" />
      <path d="M20 20l-3.5-3.5" />
    </svg>
  )
}

export function DocIcon({ className }) {
  return (
    <svg {...base} className={className}>
      <path d="M6 3h9l4 4v14H6z" />
      <path d="M14 3v5h5M9 13h6M9 17h6" />
    </svg>
  )
}

export function HomeIcon({ className }) {
  return (
    <svg {...base} className={className}>
      <path d="M3 11l9-8 9 8" />
      <path d="M5 10v10h14V10" />
      <path d="M12 20v-6" />
    </svg>
  )
}

export function CheckIcon({ className }) {
  return (
    <svg {...base} className={className}>
      <circle cx="12" cy="12" r="9" />
      <path d="M8 12.5l3 3 5-6" />
    </svg>
  )
}
