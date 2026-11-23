// Käpajälg (kasutame pildi asemel, kui loomal pole fotot)
function Paw({ className }) {
  return (
    <svg viewBox="0 0 64 64" className={className} aria-hidden="true">
      <g fill="currentColor">
        <ellipse cx="32" cy="42" rx="13" ry="11" />
        <ellipse cx="14" cy="28" rx="6" ry="8" />
        <ellipse cx="50" cy="28" rx="6" ry="8" />
        <ellipse cx="23" cy="16" rx="5.5" ry="7.5" />
        <ellipse cx="41" cy="16" rx="5.5" ry="7.5" />
      </g>
    </svg>
  )
}

export default Paw
