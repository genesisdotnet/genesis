/*
const schema = yup.object({
  firstName: yup.string().ensure().trim()
    .required('First Name is required')
    .max(255, 'First Name cannot be greater than 255 chars'),
  lastName: yup.string().ensure().trim()
    .required('Last Name is required')
    .max(255, 'Last Name cannot be greater than 255 chars'),
  userRoleId: yup.string() // not sure if this is correct or not
    .required('Role is required'),
  startDate: yup.date()
    .typeError('Hire Date is not a valid date')
    .required('Hire Date is required'),
  userId: yup.string().ensure().trim()
    .required('Email is required')
    .email('Please enter a valid email address')
    .max(255, 'Email cannot be longer than 255 chars')
});

<Row>
  <Col lg={3} md={4}>
    <InputGroup>
      <InputGroup.Prepend>
        <InputGroup.Text>
          Hire Date
        </InputGroup.Text>
      </InputGroup.Prepend>
      <InputMask mask="99/99/9999" value={values.startDate} onChange={handleChange} touched={touched} errors={errors}>
        <Form.Control
            type="text"
            name="startDate"
            value={values.startDate}
            onChange={handleChange}
            isInvalid={touched.startDate && !!errors.startDate} />
      </InputMask>
      <Form.Control.Feedback type="invalid">
        {errors.startDate}
      </Form.Control.Feedback>
    </InputGroup>
  </Col>

  <Form.Group as={Col} lg={3} md={4}>
    <Form.Control
        as="select"
        className={`sure-impact__select ${values.educationLevelValue === '' ? 'sure-impact__select-default' : ''}`}
        name="educationLevelValue"
        value={values.educationLevelValue}
        onChange={handleChange}>
      <option value="" className="sure-impact__select-default">EducationLevel</option>
      {staffEducationLevels.map(kw => (
        <option key={kw.id} value={kw.value}>
          {kw.text}
        </option>
      ))}
    </Form.Control>
  </Form.Group>

  <Form.Group as={Col} lg={6} md={12}>
    <Form.Control
        type="text"
        name="userId"
        value={values.userId}
        placeholder="Email"
        onChange={handleChange}
        isInvalid={touched.userId && !!errors.userId} />
    <Form.Control.Feedback type="invalid">
      {errors.userId}
    </Form.Control.Feedback>
  </Form.Group>

</Row>
*/