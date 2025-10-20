import React, { forwardRef } from 'react';
import HtmlEditor, {
  Toolbar, MediaResizing, ImageUpload, Item,
} from 'devextreme-react/html-editor';

const sizeValues = ['8pt', '10pt', '12pt', '14pt', '18pt', '24pt', '36pt'];
const fontValues = ['Arial', 'Courier New', 'Georgia', 'Impact', 'Lucida Console', 'Tahoma', 'Times New Roman', 'Verdana', 'Roboto'];
const headerValues = [false, 1, 2, 3, 4, 5];
const fontSizeOptions = {
  inputAttr: {
    'aria-label': 'Font size',
  },
};
const fontFamilyOptions = {
  inputAttr: {
    'aria-label': 'Font family',
  },
};
const headerOptions = {
  inputAttr: {
    'aria-label': 'Font family',
  },
};

const HTMLEditor = forwardRef((props, ref) => {
  
  return (
    <div>
      <HtmlEditor
        {...props}
        ref={ref}
        height="600px"
        // onValueChanged={(e) => console.log('e', e)}
      >
        <MediaResizing enabled={true} />
        <ImageUpload tabs={['file', 'url']} fileUploadMode="base64" />
        <Toolbar multiline={false}>
          <Item name="undo" />
          <Item name="redo" />
          <Item name="separator" />
          <Item
            name="size"
            acceptedValues={sizeValues}
            options={fontSizeOptions}
          />
          <Item
            name="font"
            acceptedValues={fontValues}
            options={fontFamilyOptions}
          />
          <Item name="separator" />
          <Item name="bold" />
          <Item name="italic" />
          <Item name="strike" />
          <Item name="underline" />
          <Item name="separator" />
          <Item name="alignLeft" />
          <Item name="alignCenter" />
          <Item name="alignRight" />
          <Item name="alignJustify" />
          <Item name="separator" />
          <Item name="orderedList" />
          <Item name="bulletList" />
          <Item name="separator" />
          <Item
            name="header"
            acceptedValues={headerValues}
            options={headerOptions}
          />
          <Item name="separator" />
          <Item name="color" />
          <Item name="background" />
          <Item name="separator" />
          <Item name="link" />
          <Item name="image" />
          <Item name="separator" />
          <Item name="clear" />
          <Item name="codeBlock" />
          <Item name="blockquote" />
          <Item name="separator" />
          <Item name="insertTable" />
          <Item name="deleteTable" />
          <Item name="insertRowAbove" />
          <Item name="insertRowBelow" />
          <Item name="deleteRow" />
          <Item name="insertColumnLeft" />
          <Item name="insertColumnRight" />
          <Item name="deleteColumn" />
        </Toolbar>
      </HtmlEditor>
    </div>
  );
})

export default HTMLEditor;
