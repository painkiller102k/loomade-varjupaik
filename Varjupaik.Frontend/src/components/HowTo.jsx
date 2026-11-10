const steps = [
  { icon: '🔍', title: 'Leia oma lemmik', text: 'Sirvi loomi ja kasuta filtreid liigi, oleku ja nime järgi.' },
  { icon: '📝', title: 'Esita taotlus', text: 'Loo konto ja kirjuta paar rida endast ning oma kodust.' },
  { icon: '🏡', title: 'Vii sõber koju', text: 'Meie töötajad vaatavad taotluse üle ja võtavad sinuga ühendust.' },
]

function HowTo() {
  return (
    <section id="kuidas" className="container section">
      <div className="steps">
        {steps.map((step, i) => (
          <div className="step" key={step.title}>
            <span className="step-number">{i + 1}</span>
            <span className="step-icon">{step.icon}</span>
            <h3>{step.title}</h3>
            <p>{step.text}</p>
          </div>
        ))}
      </div>
    </section>
  )
}

export default HowTo
